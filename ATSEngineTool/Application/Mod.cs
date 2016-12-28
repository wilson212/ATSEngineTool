using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using ATSEngineTool.Database;

namespace ATSEngineTool
{
    public static class Mod
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
        private static bool Cancelled { get; set; } = false;

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
            Cancelled = false;

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

            // Sync common sound files
            path = Path.Combine(ModPath, "sound", "truck", "common");
            if (cleanSounds && Directory.Exists(path))
            {
                ProgressUpdate(progress, $"Deleting old common sound files in {fname} folder");
                DirectoryExt.Delete(path);
            }

            if (cleanSounds || !Directory.Exists(path))
            {
                ProgressUpdate(progress, $"Copying common sound files to {fname} folder");
                DirectoryExt.Copy(Path.Combine(Program.RootPath, "sounds", "common"), path, true, true);
            }

            // Sync common noise files
            path = Path.Combine(ModPath, "sound", "truck", "noises");
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
                if (Cancelled) throw new OperationCanceledException();

                // Mod folder path to the sounds
                path = Path.Combine(ModPath, "sound", "truck", "engine", sound.FolderName);
                if (cleanSounds && Directory.Exists(path))
                {
                    ProgressUpdate(progress, $"Deleting old {sound.FolderName} sound files in {fname} folder");
                    DirectoryExt.Delete(path);
                }

                // Paths
                string intPath = Path.Combine(path, "int");
                string extPath = Path.Combine(path, "ext");

                // Interior Sounds
                string localPath = Path.Combine(Program.RootPath, "sounds", "engine", sound.FolderName, "int");
                if (Directory.Exists(localPath) && (cleanSounds || !Directory.Exists(intPath)))
                {
                    ProgressUpdate(progress, $"Copying exterior sound files to {fname} folder");
                    DirectoryExt.Copy(localPath, intPath, true, true);
                }

                // Exterior Sounds
                localPath = Path.Combine(Program.RootPath, "sounds", "engine", sound.FolderName, "ext");
                if (Directory.Exists(localPath) && (cleanSounds || !Directory.Exists(extPath)))
                {
                    ProgressUpdate(progress, $"Copying exterior sound files to {fname} folder");
                    DirectoryExt.Copy(localPath, extPath, true, true);
                }
            }
        }

        private static void TaskForm_Cancelled(object sender, CancelEventArgs e)
        {
            Cancelled = true;
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
            Cancelled = false;

            // Define our progress counter variables
            int index = 1;
            int count = 0;

            // Local variables
            string truckpath, soundPath, enginePath;
            // filename => Tuple<SoundPackage, List<Engine>>
            var soundData = new Dictionary<string, Tuple<SoundPackage, List<Engine>>>();
            // Pre-Compiled sound files
            var soundFiles = new Dictionary<SoundPackage, string[]>();
            var suitableFor = new StringBuilder();

            // Update progress
            ProgressUpdate(progress, "Generating engine def files");

            // Connect to the database
            using (AppDatabase db = new AppDatabase())
            {
                foreach (Truck truck in trucks)
                {
                    // Clear old junk
                    soundData.Clear();

                    // Check for cancellation
                    if (Cancelled) throw new OperationCanceledException();

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
                    // Add truck default sound package to the sound list (if one exists)
                    if (truck.DefaultSoundPackageId > 0)
                    {
                        // Try and fetch the default package for the truck
                        var package = db.Query<SoundPackage>(
                                "SELECT * FROM `SoundPackage` WHERE `Id`=@P0", truck.DefaultSoundPackageId
                            ).FirstOrDefault();

                        // Manually set interior and exterior sound lists
                        if (package != null)
                        {
                            soundData["interior.sii"] = new Tuple<SoundPackage, List<Engine>>(package, new List<Engine>());
                            soundData["exterior.sii"] = new Tuple<SoundPackage, List<Engine>>(package, new List<Engine>());
                        }
                    }

                    // ==============================
                    // Create engine files
                    index = 1;
                    count = truck.TruckEngines.Count();
                    foreach (Engine engine in truck.TruckEngines.Select(x => x.Engine))
                    {
                        // Update progress
                        ProgressUpdate(progress, $"Generating engine def files for \"{truck.Name}\" ({index++}/{count})");

                        // Check for cancellation
                        if (Cancelled) throw new OperationCanceledException();

                        // Convert engine to sii format
                        string contents = engine.ToSiiFormat(truck.UnitName);
                        SoundPackage sound = engine.Series.SoundPackage;

                        // === Add engine to interior sound list
                        if (!soundData.ContainsKey(sound.InteriorFileName))
                            soundData[sound.InteriorFileName] = new Tuple<SoundPackage, List<Engine>>(sound, new List<Engine>() { engine });
                        else
                            soundData[sound.InteriorFileName].Item2.Add(engine);

                        // === Add engine to exterior sound list
                        if (!soundData.ContainsKey(sound.ExteriorFileName))
                            soundData[sound.ExteriorFileName] = new Tuple<SoundPackage, List<Engine>>(sound, new List<Engine>() { engine });
                        else
                            soundData[sound.ExteriorFileName].Item2.Add(engine);

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
                    count = soundData.Count;
                    foreach (var soundPair in soundData)
                    {
                        // Check for cancellation
                        if (Cancelled) throw new OperationCanceledException();

                        // Update progress
                        ProgressUpdate(progress, $"Generating sound def files for \"{truck.Name}\" ({index++}/{count})");

                        // Clean out old junk
                        suitableFor.Clear();

                        // Define local vars
                        SoundPackage sound = soundPair.Value.Item1;
                        List<Engine> engines = soundPair.Value.Item2;
                        string root = Path.Combine(Program.RootPath, "sounds", "engine", sound.FolderName);

                        // === Add engines to the suitable_for array
                        foreach (Engine engine in engines)
                        {
                            suitableFor.AppendIf(suitableFor.Length > 0, "\t");
                            suitableFor.AppendLine($"suitable_for[]: \"{engine.UnitName}.{truck.UnitName}.engine\"");
                        }

                        // === Compile sound files if not previously compiled
                        if (!soundFiles.ContainsKey(sound))
                            soundFiles.Add(sound, sound.ToSiiFormat());

                        // Parse the sound file adding the truck name, and suitable_for items
                        int j = soundPair.Key == sound.InteriorFileName ? 0 : 1;
                        string contents = soundFiles[sound][j]
                            .Replace("{{{NAME}}}", truck.UnitName)
                            .Replace("{{{SUITABLE}}}", suitableFor.ToString().TrimEnd(Environment.NewLine.ToCharArray()));

                        // Write the sound file
                        using (FileStream str = File.Open(Path.Combine(soundPath, soundPair.Key), FileMode.Create))
                        using (StreamWriter writer = new StreamWriter(str))
                        {
                            writer.Write(contents);
                        }
                    } // End foreach sound

                    // == Transmissions?
                    var list = truck.TruckTransmissions.ToList();
                    if (list.Count > 0)
                    {
                        var transPath = Path.Combine(truckpath, "transmission");
                        if (!Directory.Exists(transPath))
                            Directory.CreateDirectory(transPath);

                        index = 1;
                        count = list.Count;
                        foreach (var transmission in list.Select(x => x.Transmission))
                        {
                            // Check for cancellation
                            if (Cancelled) throw new OperationCanceledException();

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

                // thoughts??
            }

            return soundFiles.Keys.ToArray();
        }
    }
}
