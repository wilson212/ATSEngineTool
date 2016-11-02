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
        [Column, PrimaryKey]
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
        /// Returns an array of 2 pre-compiled sii files. 0 index is the interior
        /// sii file, while index 1 is the exterior.
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
            var builder = new SiiFileBuilder();
            var objectMap = new Dictionary<string, EngineSound>();

            // Figure out the accessory name
            var name = new StringBuilder(this.UnitName);
            name.Append(".{{{NAME}}}.");
            name.AppendLineIf(type == SoundType.Exterior, "esound", "isound");

            // Write file intro
            builder.IndentStructs = false;
            builder.WriteStartDocument();

            // Write the accessory type
            builder.WriteStructStart("accessory_sound_data", name.ToString().TrimEnd());

            // Mark exterior or interior attribute
            builder.WriteAttribute("exterior_sound", type == SoundType.Exterior);
            builder.WriteLine();

            // ===
            // === Write Attributes
            // ===
            foreach (var info in SoundInfo.Attributes.Values)
            {
                WriteAttribute(info, sounds, objectMap, builder);
            }

            // Include directive.. Directives have no tabs at all!
            if (type == SoundType.Interior)
                builder.WriteInclude("/def/vehicle/truck/common_sound_int.sui");
            else
                builder.WriteInclude("/def/vehicle/truck/common_sound_ext.sui");

            // Add suitables
            builder.WriteLine();
            builder.WriteLine("{{{SUITABLE}}}");

            // Close Accessory
            builder.WriteStructEnd();
            builder.WriteLine();

            // ===
            // === Append class objects
            // ===
            foreach (var item in objectMap)
            {
                builder.WriteLine(item.Value.ToSiiFormat(item.Key, this));
            }

            // Write the include directive
            if (type == SoundType.Interior)
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
        private void WriteAttribute(SoundInfo info, 
            Dictionary<SoundAttribute, List<EngineSound>> sounds, 
            Dictionary<string, EngineSound> classMap,
            SiiFileBuilder builder)
        {
            // Only add the sound if it exists (obviously)
            if (sounds.ContainsKey(info.AttributeType))
            {
                var sound = sounds[info.AttributeType];
                if (info.IsArray)
                {
                    int i = 0;
                    string name = info.AttributeName;
                    foreach (var snd in sound)
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
                    classMap.Add(info.StructName, sound[0]);
                }

                // Trailing line?
                builder.WriteLineIf(info.AppendLineAfter);
            }
        }

        #region overrides

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

        #endregion overrides
    }
}
