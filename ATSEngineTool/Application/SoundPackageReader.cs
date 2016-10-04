using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
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
            // === Add basic engine sounds
            if (data.Start != null)
                db.EngineSounds.Add(new EngineSound(data.Start, SoundAttribute.Start, type) { Package = package });

            if (data.StartNoFuel != null)
                db.EngineSounds.Add(new EngineSound(data.StartNoFuel, SoundAttribute.StartNoFuel, type) { Package = package });

            if (data.Stop != null)
                db.EngineSounds.Add(new EngineSound(data.Stop, SoundAttribute.Stop, type) { Package = package });

            if (data.Turbo != null)
                db.EngineSounds.Add(new EngineSound(data.Turbo, SoundAttribute.Turbo, type) { Package = package });

            // === Add Engine Force Sounds
            if (data.EngineNoFuel != null)
            {
                foreach (var sound in data.EngineNoFuel)
                    db.EngineSounds.Add(new EngineSound(sound, SoundAttribute.EngineNoFuel, type) { Package = package });
            }

            if (data.Engine != null)
            {
                foreach (var sound in data.Engine)
                    db.EngineSounds.Add(new EngineSound(sound, SoundAttribute.Engine, type) { Package = package });
            }

            if (data.EngineLoad != null)
            {
                foreach (var sound in data.EngineLoad)
                    db.EngineSounds.Add(new EngineSound(sound, SoundAttribute.EngineLoad, type) { Package = package });
            }

            if (data.EngineExhaust != null)
            {
                foreach (var sound in data.EngineExhaust)
                    db.EngineSounds.Add(new EngineSound(sound, SoundAttribute.EngineExhaust, type) { Package = package });
            }

            if (data.EngineBrake != null)
            {
                foreach (var sound in data.EngineBrake)
                    db.EngineSounds.Add(new EngineSound(sound, SoundAttribute.EngineBrake, type) { Package = package });
            }

            // === Add Truck Sounds
            if (data.Horn != null)
                db.EngineSounds.Add(new EngineSound(data.Horn, SoundAttribute.Horn, type) { Package = package });

            if (data.AirHorn != null)
                db.EngineSounds.Add(new EngineSound(data.AirHorn, SoundAttribute.AirHorn, type) { Package = package });

            if (data.Reverse != null)
                db.EngineSounds.Add(new EngineSound(data.Reverse, SoundAttribute.Reverse, type) { Package = package });

            if (data.ChangeGear != null)
                db.EngineSounds.Add(new EngineSound(data.ChangeGear, SoundAttribute.ChangeGear, type) { Package = package });

            if (data.AirBrakes != null)
            {
                foreach (var sound in data.AirBrakes)
                    db.EngineSounds.Add(new EngineSound(sound, SoundAttribute.AirBrakes, type) { Package = package });
            }

            if (data.AirGears != null)
            {
                foreach (var sound in data.AirGears)
                    db.EngineSounds.Add(new EngineSound(sound, SoundAttribute.AirGears, type) { Package = package });
            }

            if (data.BlinkerOn != null)
                db.EngineSounds.Add(new EngineSound(data.BlinkerOn, SoundAttribute.BlinkerOn, type) { Package = package });

            if (data.BlinkerOff != null)
                db.EngineSounds.Add(new EngineSound(data.BlinkerOff, SoundAttribute.BlinkerOff, type) { Package = package });

            if (data.WipersUp != null)
                db.EngineSounds.Add(new EngineSound(data.WipersUp, SoundAttribute.WiperUp, type) { Package = package });

            if (data.WipersDown != null)
                db.EngineSounds.Add(new EngineSound(data.WipersDown, SoundAttribute.WiperDown, type) { Package = package });
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
