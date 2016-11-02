using System;
using System.Globalization;
using System.Text;
using ATSEngineTool.SiiEntities;
using CrossLite;
using CrossLite.CodeFirst;

namespace ATSEngineTool.Database
{
    [Table]
    public class EngineSound
    {
        #region Column Attributes

        /// <summary>
        /// Gets the Row ID for this <see cref="EngineSound"/>
        /// </summary>
        [Column, PrimaryKey]
        public int Id { get; set; }

        /// <summary>
        /// Gets or Sets the <see cref="SoundPackage.Id"/> that this 
        /// <see cref="EngineSound"/> is apart of
        /// </summary>
        [Column, Required]
        public int PackageId { get; set; }
        
        /// <summary>
        /// Gets or Sets the <see cref="SoundAttribute"/> for this sound
        /// </summary>
        [Column, Required]
        public SoundAttribute Attribute { get; set; }

        /// <summary>
        /// Gets or Sets whether this is an interior sound, exterior sound, or both
        /// </summary>
        [Column, Required]
        public SoundType Type { get; set; }

        /// <summary>
        /// Gets or Sets the sound filename this <see cref="EngineSound"/> uses
        /// </summary>
        [Column, Required]
        public string FileName { get; set; }

        /// <summary>
        /// Gets or Sets whether this sound is looped in game, or played just once
        /// </summary>
        [Column, Required, Default(false)]
        public bool Looped { get; set; }

        /// <summary>
        /// Gets or Sets whether this <see cref="EngineSound"/>'s volume changes
        /// based on the players position relevent to the sounds source.
        /// </summary>
        [Column, Required, Default(false)]
        public bool Is2D { get; set; }

        /// <summary>
        /// Gets or Sets the sounds volume in game
        /// </summary>
        [Column, Required, Default(1.0)]
        public double Volume { get; set; } = 1.0;

        /// <summary>
        /// Gets or Sets the Engine RPM's pitch reference to this sound. The
        /// greater difference between RPM and this volume, the greater the 
        /// pitch of this sound is adjusted.
        /// </summary>
        [Column, Required, Default(0)]
        public int PitchReference { get; set; } = 0;

        /// <summary>
        /// Gets or Sets the minimum engine RPM to play this sound. Set to
        /// zero to ignore minimum engine RPM
        /// </summary>
        [Column, Required, Default(0)]
        public int MinRpm { get; set; } = 0;

        /// <summary>
        /// Gets or Sets the maximum engine RPM to play this sound. Set to
        /// zero to ignore maximum engine RPM
        /// </summary>
        [Column, Required, Default(0)]
        public int MaxRpm { get; set; } = 0;

        #endregion

        #region Foreign Keys

        [InverseKey("Id")]
        [ForeignKey("PackageId",
            OnDelete = ReferentialIntegrity.Cascade,
            OnUpdate = ReferentialIntegrity.Cascade
        )]
        protected virtual ForeignKey<SoundPackage> FK_Package { get; set; }

        /// <summary>
        /// Gets or Sets the <see cref="ATSEngineTool.Database.SoundPackage"/> that 
        /// this Sound references.
        /// </summary>
        public SoundPackage Package
        {
            get
            {
                return FK_Package?.Fetch();
            }
            set
            {
                PackageId = value.Id;
                FK_Package?.Refresh();
            }
        }

        #endregion

        #region Class Properties

        /// <summary>
        /// Gets whether this sound is an engine sound, or truck sound.
        /// </summary>
        public bool IsEngineSound => SoundInfo.Attributes[this.Attribute].IsEngineSound;

        /// <summary>
        /// Gets whether this sound is an array Attribute
        /// </summary>
        public bool IsSoundArray => SoundInfo.Attributes[this.Attribute].IsArray;

        #endregion

        public EngineSound() { }

        internal EngineSound(SoundEngineData data, SoundAttribute attr, SoundType type)
        {
            this.Type = type;
            this.Attribute = attr;

            this.FileName = data.Name;
            this.Is2D = data.Is2D;
            this.Looped = data.Looped;
            this.MinRpm = (int)data.MinRPM;
            this.MaxRpm = (int)data.MaxRPM;
            this.Volume = data.Volume;
            this.PitchReference = (int)data.Pitch;
        }

        internal EngineSound(SoundData data, SoundAttribute attr, SoundType type)
        {
            this.Type = type;
            this.Attribute = attr;

            this.FileName = data.Name;
            this.Is2D = data.Is2D;
            this.Looped = data.Looped;
            this.Volume = data.Volume;
        }

        internal string ToSiiFormat(string objectName, SoundPackage package, bool indent = false)
        {
            StringBuilder builder = new StringBuilder();
            string tab1 = (indent) ? "\t" : "";
            string tab2 = tab1 + "\t";

            // Begin the accessory
            builder.Append(tab1);
            builder.AppendLineIf(this.IsEngineSound, $"sound_engine_data: {objectName}", $"sound_data: {objectName}");
            builder.Append(tab1);
            builder.AppendLine("{");

            // Figure out file path
            string file = this.FileName;
            if (this.FileName.StartsWith("@"))
            {
                string directive = this.FileName.Substring(1, 2);
                switch (directive.ToUpperInvariant())
                {
                    case "CP":
                        file = this.FileName.Replace($"@{directive}", "/sound/truck/common");
                        break;
                    case "EP":
                        file = this.FileName.Replace($"@{directive}", $"/sound/truck/engine/{package.FolderName}");
                        break;
                    default:
                        file = this.FileName.Replace($"@{directive}", $"/sound/truck/default");
                        break;
                }
            }

            // Write attributes
            builder.Append(tab2);
            builder.AppendLine($"name: \"{file}\"");
            builder.Append(tab2);
            builder.AppendLineIf(this.Looped, "looped: true", "looped: false");

            if (this.Is2D)
            {
                builder.Append(tab2);
                builder.AppendLine("is_2d: true");
            }

            if (this.IsEngineSound)
            {
                if (this.PitchReference > 0)
                {
                    builder.Append(tab2);
                    builder.AppendLine($"pitch_reference: {this.PitchReference}");
                }

                if (this.MinRpm > 0)
                {
                    builder.Append(tab2);
                    builder.AppendLine($"min_rpm: {this.MinRpm}.0");
                }

                if (this.MaxRpm > 0)
                {
                    builder.Append(tab2);
                    builder.AppendLine($"max_rpm: {this.MaxRpm}.0");
                }
            }

            if (this.Volume != 1.0)
            {
                builder.Append(tab2);
                builder.AppendLine($"volume: {this.Volume.ToString(CultureInfo.InvariantCulture)}");
            }

            // Close Accessory
            builder.Append(tab1);
            builder.AppendLine("}");
            return builder.ToString();
        }
    }
}
