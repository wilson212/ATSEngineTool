using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using ATSEngineTool.Database;

namespace ATSEngineTool
{
    public static class ModBuilder
    {
        /// <summary>
        /// Gets the Full Mod directory path
        /// </summary>
        public static string ModPath => (Program.Config.IntegrateWithMod) 
            ? Path.Combine(Program.Config.SteamPath, "SteamApps", "workshop", "content", "270880", "650050267", "latest")
            : CompilePath;

        /// <summary>
        /// Gets the directory path where def files are compiled
        /// </summary>
        public static string DefCompilePath => Path.Combine(Program.RootPath, "__compile__", "def");

        /// <summary>
        /// Gets the directory path where def files are compiled
        /// </summary>
        public static string CompilePath => Path.Combine(Program.RootPath, "__compile__");

        /// <summary>
        /// Gets or sets whether the task is cancelled
        /// </summary>
        private static bool Canceled { get; set; } = false;

        /// <summary>
        /// Removes all temporary compiled files from the Compile directory
        /// </summary>
        public static void CleanCompileDirectory(IProgress<TaskProgressUpdate> progress)
        {
            if (Directory.Exists(CompilePath))
            {
                try
                {
                    // Report
                    ProgressUpdate(progress, "Cleaning compile directory...");

                    // Delete Directory and create a new
                    DirectoryExt.Delete(CompilePath);
                    Directory.CreateDirectory(DefCompilePath);
                }
                catch (Exception e) when (e.Message.Contains("empty"))
                {
                    throw new Exception("Unable to delete Compile directory because it is open. Please close this directory and try again.", e);
                }
            }
        }

        /// <summary>
        /// Synchronizes the compiled files with the mods files.
        /// </summary>
        /// <param name="cleanEngines">If true, the mods def folder will be deleted</param>
        /// <param name="cleanSounds">
        /// If true, all sound files will be deleted from the mod and the current sound 
        /// files will be copied over to the mod.
        /// </param>
        public static void Sync(SoundPackage[] packages, bool cleanEngines, bool cleanSounds, IProgress<TaskProgressUpdate> progress)
        {
            // Set up cancellation
            TaskForm.Cancelled += TaskForm_Cancelled;
            Canceled = false;

            // Get our folder type name
            string fname = (Program.Config.IntegrateWithMod) ? "mod" : "compile";

            // Clear old def files?
            string path = Path.Combine(ModPath, "def");
            if (cleanEngines && Directory.Exists(path))
            {
                // Show Update
                ProgressUpdate(progress, $"Deleting existing {fname} def files");
                DirectoryExt.Delete(path);
            }

            // Copy the def files from the __compile__ dir to the mod
            if (Program.Config.IntegrateWithMod)
            {
                ProgressUpdate(progress, "Copying compiled def files to mod folder");
                DirectoryExt.Copy(DefCompilePath, path, true, true);
            }

            // Sync icons
            ProgressUpdate(progress, $"Copying dds graphics to {fname} folder");
            path = Path.Combine(ModPath, "material", "ui", "accessory");
            DirectoryExt.Copy(Path.Combine(Program.RootPath, "graphics"), path, true, true);

            // Sync common noise files
            path = Path.Combine(ModPath, "sound", "noises");
            if (cleanSounds && Directory.Exists(path))
            {
                ProgressUpdate(progress, $"Deleting old noise sound files in {fname} folder");
                DirectoryExt.Delete(path);
            }

            if (cleanSounds || !Directory.Exists(path))
            {
                ProgressUpdate(progress, $"Copying noise sound files to {fname} folder");
                DirectoryExt.Copy(Path.Combine(Program.RootPath, "sounds", "noises"), path, true, true);
            }

            // Sync engine sounds
            foreach (SoundPackage sound in packages)
            {
                // Check for cancellation
                if (Canceled) throw new OperationCanceledException();

                // Mod folder path to the sounds
                path = Path.Combine(ModPath, "sound", sound.PackageTypeFolderName, sound.FolderName);
                if (cleanSounds && Directory.Exists(path))
                {
                    ProgressUpdate(progress, $"Deleting old {sound.FolderName} sound files in {fname} folder");
                    DirectoryExt.Delete(path);
                }

                // Paths
                string intPath = Path.Combine(path, "int");
                string extPath = Path.Combine(path, "ext");

                // Interior Sounds
                string localPath = Path.Combine(Program.RootPath, "sounds", sound.PackageTypeFolderName, sound.FolderName, "int");
                if (Directory.Exists(localPath) && (cleanSounds || !Directory.Exists(intPath)))
                {
                    ProgressUpdate(progress, $"Copying exterior sound files to {fname} folder");
                    DirectoryExt.Copy(localPath, intPath, true, true);
                }

                // Exterior Sounds
                localPath = Path.Combine(Program.RootPath, "sounds", sound.PackageTypeFolderName, sound.FolderName, "ext");
                if (Directory.Exists(localPath) && (cleanSounds || !Directory.Exists(extPath)))
                {
                    ProgressUpdate(progress, $"Copying exterior sound files to {fname} folder");
                    DirectoryExt.Copy(localPath, extPath, true, true);
                }
            }
        }

        private static void TaskForm_Cancelled(object sender, CancelEventArgs e)
        {
            Canceled = true;
        }

        private static void ProgressUpdate(IProgress<TaskProgressUpdate> progress, string v)
        {
            // Show Update
            if (progress != null)
            {
                var update = new TaskProgressUpdate();
                update.MessageText = v;
                progress.Report(update);
            }
        }

        /// <summary>
        /// Takes the engine data from the database, and compiles the mod.
        /// </summary>
        /// <param name="trucks">A list of trucks we are compiling engines for</param>
        /// <returns>Returns an array of sound packages that are used</returns>
        public static SoundPackage[] Compile(IEnumerable<Truck> trucks, IProgress<TaskProgressUpdate> progress)
        {
            // Set up cancellation
            TaskForm.Cancelled += TaskForm_Cancelled;
            Canceled = false;

            // Define our progress counter variables
            int index = 1;
            int count = 0;

            // Local variables
            string truckpath, soundPath, enginePath;
            var usedEngineSoundPackages = new HashSet<SoundPackage>();
            var usedTruckSoundPackages = new HashSet<SoundPackage>();

            // Eager loaded cache's : PackageId => SoundPackageWrapper
            var truckSoundCache = new Dictionary<int, SoundPackageWrapper<TruckSoundPackage, TruckSound>>();
            var engineSoundCache = new Dictionary<int, SoundPackageWrapper<EngineSoundPackage, EngineSound>>();

            // Lazy loaded cache's
            var engineCache = new Dictionary<int, Engine>();
            var engineSeriesCache = new Dictionary<int, EngineSeries>();

            // Connect to the database
            using (AppDatabase db = new AppDatabase())
            {
                // Cache all sounds
                ProgressUpdate(progress, "Loading sound data into cache");
                foreach (var package in db.TruckSoundPackages)
                {
                    truckSoundCache.Add(package.Id, new SoundPackageWrapper<TruckSoundPackage, TruckSound>(package));
                }

                foreach (var package in db.EngineSoundPackages)
                {
                    engineSoundCache.Add(package.Id, new SoundPackageWrapper<EngineSoundPackage, EngineSound>(package));
                }

                // Update progress
                ProgressUpdate(progress, "Generating engine def files");

                foreach (Truck truck in trucks)
                {
                    // Check for cancellation
                    if (Canceled) throw new OperationCanceledException();

                    // Register sound package's sounds files to be copied over to the mod directory
                    var truckSoundPack = truckSoundCache[truck.SoundPackageId];
                    if (!usedTruckSoundPackages.Contains(truckSoundPack.Package))
                        usedTruckSoundPackages.Add(truckSoundPack.Package);

                    // Create a list of EngineSoundPackage id's we will need for this truck
                    var neededEngineSoundPackages = new HashSet<int>();

                    // Define paths we will use
                    truckpath = Path.Combine(DefCompilePath, "vehicle", "truck", truck.UnitName);
                    soundPath = Path.Combine(truckpath, "sound");
                    enginePath = Path.Combine(truckpath, "engine");

                    // === Create file directories
                    if (!Directory.Exists(truckpath)) Directory.CreateDirectory(truckpath);
                    if (!Directory.Exists(soundPath)) Directory.CreateDirectory(soundPath);
                    if (!Directory.Exists(enginePath)) Directory.CreateDirectory(enginePath);
                    // ===

                    // ==============================
                    // Create engine files
                    index = 1;
                    count = truck.TruckEngines.Count();
                    foreach (var truckEng in truck.TruckEngines)
                    {
                        // Check for cancellation
                        if (Canceled) throw new OperationCanceledException();

                        // Grab or load the engine from cache
                        Engine engine = GetEngine(truckEng.EngineId, db, engineCache);

                        // Update progress
                        ProgressUpdate(progress, $"Generating engine def files for \"{truck.Name}\" ({index++}/{count})");

                        // Convert engine to sii format
                        string contents = engine.ToSiiFormat(truck.UnitName);
                        var series = GetEngineSeries(engine.SeriesId, db, engineSeriesCache);
                        var package = engineSoundCache[series.SoundPackageId].Package;

                        // Register sound package as needed
                        if (!neededEngineSoundPackages.Contains(series.SoundPackageId))
                            neededEngineSoundPackages.Add(series.SoundPackageId);

                        // Register sound package's sounds files to be copied over to the mod directory
                        if (!usedEngineSoundPackages.Contains(package))
                            usedEngineSoundPackages.Add(package);

                        // Create/Open the engine.sii file, and write the new contents
                        using (FileStream str = File.Open(Path.Combine(enginePath, engine.FileName), FileMode.Create))
                        using (StreamWriter writer = new StreamWriter(str))
                        {
                            writer.Write(contents);
                        }
                    }

                    // ==============================
                    // Create sound files for this truck
                    index = 1;
                    count = neededEngineSoundPackages.Count;
                    foreach (var packId in neededEngineSoundPackages)
                    {
                        // Check for cancellation
                        if (Canceled) throw new OperationCanceledException();

                        // Update progress
                        ProgressUpdate(progress, $"Generating sound def files for \"{truck.Name}\" ({index++}/{count})");

                        // Define local vars
                        var engineSoundPack = engineSoundCache[packId];

                        // Write the interior sound file
                        using (FileStream str = File.Open(Path.Combine(soundPath, engineSoundPack.Package.InteriorFileName), FileMode.Create))
                        using (StreamWriter writer = new StreamWriter(str))
                        {
                            writer.Write(CompileSoundFile(SoundLocation.Interior, truck, engineSoundPack, truckSoundPack));
                        }

                        // Write the exterior sound file
                        using (FileStream str = File.Open(Path.Combine(soundPath, engineSoundPack.Package.ExteriorFileName), FileMode.Create))
                        using (StreamWriter writer = new StreamWriter(str))
                        {
                            writer.Write(CompileSoundFile(SoundLocation.Exterior, truck, engineSoundPack, truckSoundPack));
                        }
                    } // End foreach sound

                    // === Sound overrides?
                    var setting = truck.SoundSetting.FirstOrDefault();
                    if (setting != null)
                    {
                        // Define local vars
                        var engineSoundPack = engineSoundCache[setting.EngineSoundPackageId];

                        // Register sound package's sounds files to be copied over to the mod directory
                        if (!usedEngineSoundPackages.Contains(engineSoundPack.Package))
                            usedEngineSoundPackages.Add(engineSoundPack.Package);

                        // Write the interior sound file
                        using (FileStream str = File.Open(Path.Combine(soundPath, "interior.sii"), FileMode.Create))
                        using (StreamWriter writer = new StreamWriter(str))
                        {
                            writer.Write(CompileSoundFile(SoundLocation.Interior, truck, engineSoundPack, truckSoundPack, "std"));
                        }

                        // Write the exterior sound file
                        using (FileStream str = File.Open(Path.Combine(soundPath, "exterior.sii"), FileMode.Create))
                        using (StreamWriter writer = new StreamWriter(str))
                        {
                            writer.Write(CompileSoundFile(SoundLocation.Exterior, truck, engineSoundPack, truckSoundPack, "std"));
                        }
                    }

                    // == Transmissions?
                    var list = truck.TruckTransmissions.ToList();
                    if (list.Count > 0)
                    {
                        // Ensure transmission directory exists
                        var transPath = Path.Combine(truckpath, "transmission");
                        if (!Directory.Exists(transPath))
                            Directory.CreateDirectory(transPath);

                        index = 1;
                        count = list.Count;
                        foreach (var transmission in list.Select(x => x.Transmission))
                        {
                            // Check for cancellation
                            if (Canceled) throw new OperationCanceledException();

                            // Update progress
                            ProgressUpdate(progress, $"Generating transmission def files for \"{truck.Name}\" ({index++}/{count})");
                            string contents = transmission.ToSiiFormat(truck.UnitName);

                            // Create/Open the engine.sii file, and write the new contents
                            string path = Path.Combine(transPath, transmission.FileName);
                            using (FileStream str = File.Open(path, FileMode.Create))
                            using (StreamWriter writer = new StreamWriter(str))
                            {
                                writer.Write(contents);
                            }
                        }
                    }

                } // End foreach truck
            }

            var returnList = new List<SoundPackage>(usedEngineSoundPackages);
            returnList.AddRange(usedTruckSoundPackages);
            return returnList.ToArray();
        }

        /// <summary>
        /// Gets an <see cref="Engine"/> with the specified key from the cache. If the 
        /// <see cref="Engine"/> is not found in the cache, then it is fetched from the
        /// database and stored in the cache.
        /// </summary>
        private static Engine GetEngine(int engineId, AppDatabase db, Dictionary<int, Engine> cache)
        {
            Engine engine = null;
            if (!cache.TryGetValue(engineId, out engine))
            {
                engine = db.Query<Engine>("SELECT * FROM `Engine` WHERE `Id`=@P0", engineId).FirstOrDefault();
                cache.Add(engineId, engine);
            }

            return engine;
        }

        /// <summary>
        /// Gets an <see cref="EngineSeries"/> with the specified key from the cache. If the 
        /// <see cref="EngineSeries"/> is not found in the cache, then it is fetched from the
        /// database and stored in the cache.
        /// </summary>
        private static EngineSeries GetEngineSeries(int seriesId, AppDatabase db, Dictionary<int, EngineSeries> cache)
        {
            EngineSeries series = null;
            if (!cache.TryGetValue(seriesId, out series))
            {
                series = db.Query<EngineSeries>("SELECT * FROM `EngineSeries` WHERE `Id`=@P0", seriesId).FirstOrDefault();
                cache.Add(seriesId, series);
            }

            return series;
        }

        /// <summary>
        /// Sound File compiler
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static string CompileSoundFile(
            SoundLocation type,
            Truck truck,
            SoundPackageWrapper<EngineSoundPackage, EngineSound> enginePackage,
            SoundPackageWrapper<TruckSoundPackage, TruckSound> truckPackage,
            string unitName = null)
        {
            // Local variables
            var builder = new SiiFileBuilder();
            var objectMap = new Dictionary<string, Sound>();
            var engineSounds = enginePackage.GetSoundsByLocation(type);
            var truckSounds = truckPackage.GetSoundsByLocation(type);

            // Figure out the accessory name
            var name = new StringBuilder(unitName ?? enginePackage.Package.UnitName);
            name.Append($".{truck.UnitName}.");
            name.AppendLineIf(type == SoundLocation.Exterior, "esound", "isound");

            // Write file intro
            builder.IndentStructs = false;
            builder.WriteStartDocument();

            // Write the accessory type
            builder.WriteStructStart("accessory_sound_data", name.ToString().TrimEnd());

            // Mark exterior or interior attribute
            builder.WriteAttribute("exterior_sound", type == SoundLocation.Exterior);
            builder.WriteLine();

            // ===
            // === Write Engine Attributes
            // ===
            foreach (var info in SoundInfo.Attributes.Values)
            {
                // Only engine sounds
                if (info.SoundType != SoundType.Engine)
                    continue;

                WriteAttribute(info, engineSounds, objectMap, builder);
            }

            // ===
            // === Write Truck Attributes
            // ===
            foreach (var info in SoundInfo.Attributes.Values)
            {
                // Only truck sounds
                if (info.SoundType != SoundType.Truck)
                    continue;

                WriteAttribute(info, truckSounds, objectMap, builder);
            }

            // Include directive.. Directives have no tabs at all!
            if (type == SoundLocation.Interior)
                builder.WriteInclude("/def/vehicle/truck/common_sound_int.sui");
            else
                builder.WriteInclude("/def/vehicle/truck/common_sound_ext.sui");

            // Close Accessory
            builder.WriteStructEnd();
            builder.WriteLine();

            // ===
            // === Append class objects
            // ===
            foreach (var item in objectMap)
            {
                // Get sound package
                SoundPackage package = (item.Value.SoundType == SoundType.Engine)
                    ? enginePackage.Package as SoundPackage
                    : truckPackage.Package as SoundPackage;

                // Add sound object
                item.Value.AppendTo(builder, item.Key, package);
            }

            // Write the include directive
            if (type == SoundLocation.Interior)
                builder.WriteInclude("/def/vehicle/truck/common_sound_int_data.sui");
            else
                builder.WriteInclude("/def/vehicle/truck/common_sound_ext_data.sui");

            // Close SiiNUnit
            builder.WriteEndDocument();
            return builder.ToString();
        }

        /// <summary>
        /// Writes an attribute to the StringBuilder if the attribute type exists in the sounds list.
        /// </summary>
        /// <param name="sounds">The list of sound attributes and their sounds for this package</param>
        /// <param name="classMap">A ruuning list of objects that will be later written to the buffer.</param>
        /// <param name="builder">The current string buffer</param>
        private static void WriteAttribute<TSound>(SoundInfo info,
            Dictionary<SoundAttribute, List<TSound>> sounds,
            Dictionary<string, Sound> classMap,
            SiiFileBuilder builder) where TSound : Sound
        {
            // Only add the sound if it exists (obviously)
            List<TSound> soundList;
            if (sounds.TryGetValue(info.AttributeType, out soundList))
            {
                if (info.IsArray)
                {
                    int i = 0;
                    string name = info.AttributeName;
                    foreach (var snd in soundList)
                    {
                        // Write attribute line
                        string sname = info.StructName + i++;
                        builder.WriteLineIf(info.Indexed, $"{name}[{i - 1}]: {sname}", $"{name}[]: {sname}");

                        // Add to classmap
                        classMap.Add(sname, snd);
                    }
                }
                else
                {
                    // Write attribute line
                    builder.WriteAttribute(info.AttributeName, info.StructName, false);

                    // Add to classmap
                    classMap.Add(info.StructName, soundList[0]);
                }

                // Trailing line?
                builder.WriteLineIf(info.AppendLineAfter);
            }
        }
    }
}
