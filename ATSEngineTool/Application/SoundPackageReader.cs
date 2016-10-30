using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
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
        /// Creates a new instance of SoundPackageReader
        /// </summary>
        /// <param name="filePath">The full filepath to the archive</param>
        public SoundPackageReader(string filePath)
        {
            FileStream stream = new FileStream(filePath, FileMode.Open);
            Archive = new ZipArchive(stream, ZipArchiveMode.Read, false);
            IsOpen = true;
        }

        /// <summary>
        /// Creates a new instance of SoundPackageReader
        /// </summary>
        /// <param name="stream">An open stream to the archive</param>
        public SoundPackageReader(Stream stream)
        {
            Archive = new ZipArchive(stream, ZipArchiveMode.Read, false);
            IsOpen = true;
        }

        /// <summary>
        /// Fetches the manifest from the sound package
        /// </summary>
        /// <param name="name">An out variable that returns the package name</param>
        /// <returns></returns>
        public SoundPackManifest GetManifest(out string name)
        {
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
            name = new List<string>(document.Definitions.Keys).First();
            return document.GetDefinition<SoundPackManifest>(name);
        }

        /// <summary>
        /// Fetches the specified sound accessory
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public AccessorySoundData GetSoundFile(SoundType type)
        {
            // Load the file entry from the archive
            var entry = Archive.GetEntry(type == SoundType.Interior ? "interior.sii" : "exterior.sii");
            if (entry == null) return null;

            // Parse the sii file
            var document = new SiiDocument(typeof(AccessorySoundData), typeof(SoundData), typeof(SoundEngineData));
            using (StreamReader reader = new StreamReader(entry.Open()))
                document.Load(reader.ReadToEnd().Trim());

            // return the main object
            var key = new List<string>(document.Definitions.Keys).First();
            return document.GetDefinition<AccessorySoundData>(key);
        }

        /// <summary>
        /// Imports the sound accessory objects into the AppDatabase
        /// </summary>
        /// <param name="db">An open AppData.db connection</param>
        /// <param name="package">The <see cref="SoundPackage"/> that will own these accessory objects</param>
        /// <param name="data">The accessory data object</param>
        /// <param name="type">The sound type</param>
        public void ImportSounds(AppDatabase db, SoundPackage package, AccessorySoundData data, SoundType type)
        {
            // Grab all properties that have the SoundAttribute attribute
            List<PropertyInfo> result = typeof(AccessorySoundData).GetProperties()
                .Where(prop => prop.IsDefined(typeof(SoundAttributeAttribute), false))
                .ToList();

            // Using reflection, we will now loop through each property
            // with the SoundAttribute attribute, and create an EngineSound
            // entity using that data.
            foreach (var prop in result)
            {
                // Define local vars
                SoundAttribute attr = prop.GetCustomAttribute<SoundAttributeAttribute>().Attribute;
                SoundInfo info = SoundInfo.Attributes[attr];

                if (info.IsArray)
                {
                    if (info.IsEngineSound)
                    {
                        var values = ((SoundEngineData[])prop.GetValue(data) ?? new SoundEngineData[] { });
                        foreach (var sound in values)
                            db.EngineSounds.Add(new EngineSound(sound, attr, type) { Package = package });
                    }
                    else
                    {
                        var values = ((SoundData[])prop.GetValue(data) ?? new SoundData[] { });
                        foreach (var sound in values)
                            db.EngineSounds.Add(new EngineSound(sound, attr, type) { Package = package });
                    }
                }
                else
                {
                    if (info.IsEngineSound)
                    {
                        var sound = (SoundEngineData)prop.GetValue(data);
                        if (sound != null)
                            db.EngineSounds.Add(new EngineSound(sound, attr, type) { Package = package });
                    }
                    else
                    {
                        var sound = (SoundData)prop.GetValue(data);
                        if (sound != null)
                            db.EngineSounds.Add(new EngineSound(sound, attr, type) { Package = package });
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
