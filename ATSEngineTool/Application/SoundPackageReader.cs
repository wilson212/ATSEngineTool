using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using ATSEngineTool.Database;
using ATSEngineTool.SiiEntities;
using Sii;

namespace ATSEngineTool
{
    /// <summary>
    /// A class used to read .espack (EngineSoundPACKages) files
    /// </summary>
    public class SoundPackageReader : IDisposable
    {
        /// <summary>
        /// The .espack archive
        /// </summary>
        protected ZipArchive Archive { get; set; }

        /// <summary>
        /// Returns whether the internal sound package archive is open
        /// </summary>
        public bool IsOpen { get; protected set; } = false;

        /// <summary>
        /// Indicates the archive sound type
        /// </summary>
        public SoundType Type { get; set; }

        /// <summary>
        /// Cache of this Sound Package's name
        /// </summary>
        protected string PackageName { get; set; } = String.Empty;

        /// <summary>
        /// Cache of this Sound Package's manifest.sii
        /// </summary>
        protected SoundPackManifest Manifest { get; set; }

        /// <summary>
        /// Cache of this Sound Package's interior.sii
        /// </summary>
        protected AccessorySoundData Interior { get; set; }

        /// <summary>
        /// Cache of this Sound Package's exterior.sii
        /// </summary>
        protected AccessorySoundData Exterior { get; set; }

        /// <summary>
        /// Creates a new instance of SoundPackageReader
        /// </summary>
        /// <param name="filePath">The full filepath to the archive</param>
        public SoundPackageReader(string filePath, SoundType type)
        {
            FileStream stream = new FileStream(filePath, FileMode.Open);
            Archive = new ZipArchive(stream, ZipArchiveMode.Read, false);
            Type = type;
            IsOpen = true;
        }

        /// <summary>
        /// Creates a new instance of SoundPackageReader
        /// </summary>
        /// <param name="stream">An open stream to the archive</param>
        public SoundPackageReader(Stream stream, SoundType type)
        {
            Archive = new ZipArchive(stream, ZipArchiveMode.Read, false);
            Type = type;
            IsOpen = true;
        }

        /// <summary>
        /// Fetches the manifest from the sound package
        /// </summary>
        /// <param name="name">An out variable that returns the package name</param>
        /// <returns></returns>
        public SoundPackManifest GetManifest(out string name)
        {
            // Check cache
            if (Manifest != null)
            {
                name = PackageName;
                return Manifest;
            }

            // Load the file entry from the archive
            var entry = Archive.GetEntry("manifest.sii");
            if (entry == null)
            {
                name = string.Empty;
                return null;
            }

            // Parse the sii file
            var document = new SiiDocument(typeof(SoundPackManifest));
            using (StreamReader reader = new StreamReader(entry.Open()))
                document.Load(reader.ReadToEnd().Trim());

            // Grab the manifest object
            PackageName = name = new List<string>(document.Definitions.Keys).First();
            return document.GetDefinition<SoundPackManifest>(PackageName);
        }

        /// <summary>
        /// Fetches the specified sound accessory
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public AccessorySoundData GetSoundFile(SoundLocation type)
        {
            // Check cache
            switch (type)
            {
                case SoundLocation.Interior: if (Interior != null) return Interior; break;
                case SoundLocation.Exterior: if (Exterior != null) return Exterior; break;
            }

            // Load the file entry from the archive
            var entry = Archive.GetEntry(type == SoundLocation.Interior ? "interior.sii" : "exterior.sii");
            if (entry == null) return null;

            // Parse the sii file
            var document = new SiiDocument(typeof(AccessorySoundData), typeof(SoundData), typeof(SoundEngineData));
            using (StreamReader reader = new StreamReader(entry.Open()))
                document.Load(reader.ReadToEnd().Trim());

            // return the main object
            var key = new List<string>(document.Definitions.Keys).First();
            var item = document.GetDefinition<AccessorySoundData>(key);

            // Cache the sound data
            if (type == SoundLocation.Interior)
                Interior = item;
            else
                Exterior = item;

            // return
            return item;
        }

        /// <summary>
        /// Installs this <see cref="SoundPackage"/> onto the file system and database.
        /// </summary>
        /// <param name="db"></param>
        /// <param name="package"></param>
        /// <param name="folderPath"></param>
        /// <param name="deleteExisting"></param>
        public void InstallPackage(AppDatabase db, SoundPackage package, string folderPath, bool deleteExisting = false)
        {
            // Grab all properties that have the SoundAttribute attribute
            List<PropertyInfo> properties = typeof(AccessorySoundData).GetProperties()
                .Where(prop => prop.IsDefined(typeof(SoundAttributeAttribute), false))
                .ToList();

            // Get sound data
            var interior = (Interior == null) ? GetSoundFile(SoundLocation.Interior) : Interior;
            var exterior = (Exterior == null) ? GetSoundFile(SoundLocation.Exterior) : Exterior;

            // Import sounds
            var files = new List<string>();
            ImportSounds(db, package, interior, SoundLocation.Interior, properties, files);
            ImportSounds(db, package, exterior, SoundLocation.Exterior, properties, files);

            // Extract sound files used
            foreach (string file in files)
            {
                // Format the file name, removing any @ directives
                var filename = Regex.Replace(file, "^@(?<Code>[A-Z]+)/", "").TrimStart(new[] { '/' });
                var entry = Archive.GetEntry(filename);

                // Ignore missing files
                if (entry != null)
                {
                    // Ensure directory existance
                    var fileExtractPath = Path.Combine(folderPath, filename.Replace('/', Path.DirectorySeparatorChar));
                    var dirName = Path.GetDirectoryName(fileExtractPath);
                    if (!Directory.Exists(dirName))
                    {
                        Directory.CreateDirectory(dirName);
                    }

                    // Extract audio file
                    entry.ExtractToFile(fileExtractPath, deleteExisting);
                }
            }

            // Incase there were no sound files, ensure sound directory exists
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            // Extract sound files and manifest
            Archive.GetEntry("manifest.sii").ExtractToFile(Path.Combine(folderPath, "manifest.sii"), true);
            Archive.GetEntry("interior.sii").ExtractToFile(Path.Combine(folderPath, "interior.sii"), true);
            Archive.GetEntry("exterior.sii").ExtractToFile(Path.Combine(folderPath, "exterior.sii"), true);
        }

        /// <summary>
        /// Imports the sound accessory objects into the AppDatabase
        /// </summary>
        /// <param name="db">An open AppData.db connection</param>
        /// <param name="package">The <see cref="SoundPackage"/> that will own these accessory objects</param>
        /// <param name="data">The accessory data object</param>
        /// <param name="location">The sound type</param>
        protected void ImportSounds(
            AppDatabase db, 
            SoundPackage package, 
            AccessorySoundData data, 
            SoundLocation location, 
            List<PropertyInfo> properties,
            List<string> files)
        {
            // Using reflection, we will now loop through each property
            // with the SoundAttribute attribute, and create an EngineSound
            // entity using that data.
            foreach (var prop in properties)
            {
                // Define local vars
                SoundAttribute attr = prop.GetCustomAttribute<SoundAttributeAttribute>().Attribute;
                SoundInfo info = SoundInfo.Attributes[attr];

                // Skip if wrong sound type
                if (info.SoundType != Type)
                    continue;

                if (Type == SoundType.Engine)
                {
                    if (info.IsArray)
                    {
                        if (info.IsEngineSoundData)
                        {
                            var values = ((SoundEngineData[])prop.GetValue(data) ?? new SoundEngineData[] { });
                            foreach (var sound in values)
                            {
                                db.EngineSounds.Add(new EngineSound(sound, attr, location) { PackageId = package.Id });
                                files.Add(sound.Name);
                            }
                        }
                        else
                        {
                            var values = ((SoundData[])prop.GetValue(data) ?? new SoundData[] { });
                            foreach (var sound in values)
                            {
                                db.EngineSounds.Add(new EngineSound(sound, attr, location) { PackageId = package.Id });
                                files.Add(sound.Name);
                            }
                        }
                    }
                    else
                    {
                        if (info.IsEngineSoundData)
                        {
                            var sound = (SoundEngineData)prop.GetValue(data);
                            if (sound != null)
                            {
                                db.EngineSounds.Add(new EngineSound(sound, attr, location) { PackageId = package.Id });
                                files.Add(sound.Name);
                            }
                        }
                        else
                        {
                            var sound = (SoundData)prop.GetValue(data);
                            if (sound != null)
                            {
                                db.EngineSounds.Add(new EngineSound(sound, attr, location) { PackageId = package.Id });
                                files.Add(sound.Name);
                            }
                        }
                    }
                }
                else if (Type == SoundType.Truck)
                {
                    if (info.IsArray)
                    {
                        var values = ((SoundData[])prop.GetValue(data) ?? new SoundData[] { });
                        foreach (var sound in values)
                        {
                            db.TruckSounds.Add(new TruckSound(sound, attr, location) { PackageId = package.Id });
                            files.Add(sound.Name);
                        }
                    }
                    else
                    {
                        var sound = (SoundData)prop.GetValue(data);
                        if (sound != null)
                        {
                            db.TruckSounds.Add(new TruckSound(sound, attr, location) { PackageId = package.Id });
                            files.Add(sound.Name);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Extracts the contents of the sound package archive into the specified folder.
        /// </summary>
        /// <param name="folderPath"></param>
        /// <param name="deleteExisting"></param>
        public void ExtractToDirectory(string folderPath, bool deleteExisting = false)
        {
            if (deleteExisting && Directory.Exists(folderPath))
                DirectoryExt.Delete(folderPath);

            Archive.ExtractToDirectory(folderPath);
        }

        /// <summary>
        /// Closes the open sound package archive
        /// </summary>
        public void CloseArchive()
        {
            if (IsOpen)
            {
                Archive.Dispose();
                IsOpen = false;
            }
        }

        public void Dispose()
        {
            if (IsOpen)
            {
                Archive.Dispose();
                IsOpen = false;
            }
        }
    }
}
