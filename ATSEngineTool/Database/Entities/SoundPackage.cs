using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CrossLite;
using CrossLite.CodeFirst;

namespace ATSEngineTool.Database
{
    [Table]
    public class SoundPackage
    {
        /// <summary>
        /// Gets or Sets the Unique id for this entity
        /// </summary>
        [Column, PrimaryKey, AutoIncrement]
        public int Id { get; protected set; }

        /// <summary>
        /// Gets or Sets the unique name for this Sound
        /// </summary>
        [Column, Required, Unique]
        public string Name { get; set; }

        /// <summary>
        /// Gets or Sets the folder name that this sound package is located in.
        /// </summary>
        [Column, Required, Unique, Collation(Collation.NoCase)]
        public string FolderName { get; set; }

        /// <summary>
        /// Gets or Sets the filename that this <see cref="SoundPackage"/> will use
        /// when generating the interior sound file.
        /// </summary>
        [Column, Required]
        public string InteriorFileName { get; set; }

        /// <summary>
        /// Gets or Sets the filename that this <see cref="SoundPackage"/> will use
        /// when generating the exterior sound file.
        /// </summary>
        [Column, Required]
        public string ExteriorFileName { get; set; }

        /// <summary>
        /// Gets or Sets the unique name for this Sound
        /// </summary>
        [Column, Required]
        public string UnitName { get; set; }

        /// <summary>
        /// Gets or Sets the author of this sound package
        /// </summary>
        [Column, Required, Default("")]
        public string Author { get; set; }

        /// <summary>
        /// Gets or Sets the package version number
        /// </summary>
        [Column, Required, Default(1.0)]
        public decimal Version { get; set; }

        /// <summary>
        /// Gets a list of <see cref="EngineSeries"/> that reference this 
        /// <see cref="SoundPackage"/>
        /// </summary>
        /// <remarks>
        /// A lazy loaded enumeration that fetches all EngineSeries
        /// that are bound by the foreign key and this SoundPackage.Id.
        /// </remarks>
        public virtual IEnumerable<EngineSeries> Series { get; set; }

        /// <summary>
        /// Gets a list of <see cref="EngineSound"/> entities that reference this 
        /// <see cref="SoundPackage"/>
        /// </summary>
        public virtual IEnumerable<EngineSound> EngineSounds { get; set; }

        /// <summary>
        /// Returns the relative sounds filepath
        /// </summary>
        public string FolderPath
        {
            get
            {
                bool isSCSsound = this.FolderName.Equals("default", StringComparison.InvariantCultureIgnoreCase);
                return (isSCSsound) ? "/sound/truck/default/" : $"/sound/truck/engine/{this.FolderName}/";
            }
        }

        private static string SingleTab = "";
        private static string DoubleTab = "\t";


        public override string ToString() => Name;

        public override bool Equals(object obj)
        {
            if (obj is SoundPackage)
            {
                SoundPackage compare = (SoundPackage)obj;
                return compare.Id == this.Id;
            }
            return false;
        }

        public override int GetHashCode() => this.Id.GetHashCode();

        /// <summary>
        /// Returns an array of 2 pre-compiled sii files.
        /// </summary>
        /// <returns></returns>
        public string[] ToSiiFormat()
        {
            return new string[]
            {
                CompileSiiFile(SoundType.Interior),
                CompileSiiFile(SoundType.Exterior)
            };
        }

        /// <summary>
        /// Sii File compiler
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        protected string CompileSiiFile(SoundType type)
        {
            // name => listOfObjects
            var sounds = new Dictionary<SoundAttribute, List<EngineSound>>();
            foreach (var sound in this.EngineSounds.Where(x => x.Type == type))
            {
                if (!sounds.ContainsKey(sound.Attribute))
                    sounds.Add(sound.Attribute, new List<EngineSound>());

                sounds[sound.Attribute].Add(sound);
            }

            // Local variables
            string name = this.UnitName + ".{{{NAME}}}." + ((type == SoundType.Interior) ? "isound" : "esound");
            StringBuilder builder = new StringBuilder();
            // objectName => Sound
            var classMap = new Dictionary<string, EngineSound>();

            // Write file intro
            builder.AppendLine("SiiNunit");
            builder.AppendLine("{");

            // Begin the accessory
            builder.Append(SingleTab);
            builder.AppendLine($"accessory_sound_data : {name}");
            builder.Append(SingleTab);
            builder.AppendLine("{");

            // Mark exterior or interior
            builder.Append(DoubleTab);
            builder.AppendLineIf(type == SoundType.Interior, "exterior_sound: false");
            builder.AppendLineIf(type == SoundType.Exterior, "exterior_sound: true");
            builder.AppendLine();

            // Start building attributes
            WriteAttribute(SoundAttribute.Start, ".start", sounds, classMap, builder);
            WriteAttribute(SoundAttribute.StartNoFuel, ".startbad", sounds, classMap, builder);
            WriteAttribute(SoundAttribute.Stop, ".stop", sounds, classMap, builder);
            builder.AppendLine();

            // Engine specific
            WriteAttribute(SoundAttribute.Engine, ".e", sounds, classMap, builder);
            WriteAttribute(SoundAttribute.EngineLoad, ".el", sounds, classMap, builder);
            WriteAttribute(SoundAttribute.EngineNoFuel, ".enf", sounds, classMap, builder);
            WriteAttribute(SoundAttribute.EngineExhaust, ".ee", sounds, classMap, builder);
            // No blank line needed (already there)

            // Turbo
            WriteAttribute(SoundAttribute.Turbo, ".t", sounds, classMap, builder);
            builder.AppendLine();

            // Air gears and brakes
            WriteAttribute(SoundAttribute.AirGears, ".airgear", sounds, classMap, builder, indexArrays: true);
            WriteAttribute(SoundAttribute.AirBrakes, ".airbrake", sounds, classMap, builder, indexArrays: true);
            // No blank line needed (already there)

            // Engine Brake
            WriteAttribute(SoundAttribute.EngineBrake, ".eb", sounds, classMap, builder);
            // No blank line needed (already there)

            // Truck sounds
            WriteAttribute(SoundAttribute.Horn, ".horn", sounds, classMap, builder);
            WriteAttribute(SoundAttribute.AirHorn, ".ahorn", sounds, classMap, builder);
            WriteAttribute(SoundAttribute.Reverse, ".reverse", sounds, classMap, builder);
            builder.AppendLine();

            // More Truck sounds
            WriteAttribute(SoundAttribute.BlinkerOn, ".blinker_on", sounds, classMap, builder);
            WriteAttribute(SoundAttribute.BlinkerOff, ".blinker_off", sounds, classMap, builder);
            WriteAttribute(SoundAttribute.WiperUp, ".wipers_up", sounds, classMap, builder);
            WriteAttribute(SoundAttribute.WiperDown, ".wipers_down", sounds, classMap, builder);
            builder.AppendLine();

            // Include directive
            builder.AppendLineIf(type == SoundType.Interior, "@include \"/def/vehicle/truck/common_sound_int.sui\"");
            builder.AppendLineIf(type == SoundType.Exterior, "@include \"/def/vehicle/truck/common_sound_ext.sui\"");
            builder.AppendLine();
            builder.Append(DoubleTab);
            builder.AppendLine("{{{SUITABLE}}}");
            builder.AppendLine();

            // Close Accessory
            builder.Append(SingleTab);
            builder.AppendLine("}");
            builder.AppendLine();

            // Append class objects
            foreach (var item in classMap)
            {
                builder.AppendLine(item.Value.ToSiiFormat(item.Key, this));
            }

            // Close SiiNUnit
            builder.AppendLineIf(type == SoundType.Interior, "@include \"/def/vehicle/truck/common_sound_int_data.sui\"");
            builder.AppendLineIf(type == SoundType.Exterior, "@include \"/def/vehicle/truck/common_sound_ext_data.sui\"");
            builder.AppendLine("}");
            return builder.ToString().TrimEnd();
        }

        /// <summary>
        /// Writes an attribute to the StringBuilder if it exists in the sounds list.
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="objectName"></param>
        /// <param name="sounds"></param>
        /// <param name="classMap"></param>
        /// <param name="builder"></param>
        /// <param name="appendLineOnSingle"></param>
        private void WriteAttribute(SoundAttribute attribute, 
                                    string objectName,
                                    Dictionary<SoundAttribute, List<EngineSound>> sounds, 
                                    Dictionary<string, EngineSound> classMap,
                                    StringBuilder builder,
                                    bool appendLineOnSingle = false,
                                    bool indexArrays = false)
        {
            // Only add the sound if it exists (obviously)
            if (sounds.ContainsKey(attribute))
            {
                var sound = sounds[attribute];
                string name = EngineSound.AttributeNames[attribute];
                if (sound[0].IsSoundArray)
                {
                    // Apply ordering if need be
                    //if (EngineSound.IsEngineSoundType(attribute) && sound.Count > 1)
                        //sound = sound.OrderBy(x => x.PitchReference).ToList();

                    int i = 0;
                    foreach (var snd in sound)
                    {
                        string cname = objectName + i++;
                        builder.Append(DoubleTab);
                        builder.AppendLineIf(indexArrays, $"{name}[{i-1}]: {cname}", $"{name}[]: {cname}");
                        classMap.Add(cname, snd);
                    }
                    builder.AppendLine();
                }
                else
                {
                    builder.Append(DoubleTab);
                    builder.AppendLine($"{name}: {objectName}");
                    builder.AppendLineIf(appendLineOnSingle);
                    classMap.Add(objectName, sound[0]);
                }
            }
        }
    }
}
