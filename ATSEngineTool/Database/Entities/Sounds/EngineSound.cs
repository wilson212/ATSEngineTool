using System;
using ATSEngineTool.SiiEntities;
using CrossLite;
using CrossLite.CodeFirst;

namespace ATSEngineTool.Database
{
    [Table]
    public class EngineSound : Sound
    {
        #region Column Attributes

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

        [InverseKey(nameof(Database.SoundPackage.Id))]
        [ForeignKey(nameof(PackageId),
            OnDelete = ReferentialIntegrity.Cascade,
            OnUpdate = ReferentialIntegrity.Cascade
        )]
        protected virtual ForeignKey<EngineSoundPackage> FK_Package { get; set; }

        /// <summary>
        /// Gets or Sets the <see cref="ATSEngineTool.Database.SoundPackage"/> that 
        /// this Sound references.
        /// </summary>
        public EngineSoundPackage Package
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

        public override SoundType SoundType => SoundType.Engine;

        /// <summary>
        /// Creates a new instance of <see cref="EngineSound"/>
        /// </summary>
        public EngineSound() { }

        /// <summary>
        /// Creates a new instance of <see cref="EngineSound"/>
        /// </summary>
        public EngineSound(SoundEngineData data, SoundAttribute attr, SoundLocation location)
        {
            this.Location = location;
            this.Attribute = attr;

            this.FileName = data.Name;
            this.Is2D = data.Is2D;
            this.Looped = data.Looped;
            this.MinRpm = (int)data.MinRPM;
            this.MaxRpm = (int)data.MaxRPM;
            this.Volume = data.Volume;
            this.PitchReference = (int)data.Pitch;
        }

        /// <summary>
        /// Creates a new instance of <see cref="EngineSound"/>
        /// </summary>
        public EngineSound(SoundData data, SoundAttribute attr, SoundLocation location)
        {
            this.Location = location;
            this.Attribute = attr;

            this.FileName = data.Name;
            this.Is2D = data.Is2D;
            this.Looped = data.Looped;
            this.Volume = data.Volume;
        }

        /// <summary>
        /// Appends this <see cref="EngineSound"/> to an open <see cref="SiiFileBuilder"/> object
        /// </summary>
        public override void AppendTo(SiiFileBuilder builder, string objectName, SoundPackage package)
        {
            // Begin the accessory
            builder.WriteStructStart("sound_engine_data", objectName);

            // Write attributes
            builder.WriteAttribute("name", GetParsedFileNamePath(package));
            builder.WriteAttribute("looped", this.Looped);

            if (this.Is2D)
                builder.WriteAttribute("is_2d", true);

            if (this.PitchReference > 0)
                builder.WriteAttribute("pitch_reference", this.PitchReference);

            if (this.MinRpm > 0)
                builder.WriteAttribute("min_rpm", (decimal)this.MinRpm);

            if (this.MaxRpm > 0)
                builder.WriteAttribute("max_rpm", (decimal)this.MaxRpm);

            if (this.Volume != 1.0)
                builder.WriteAttribute("volume", this.Volume);

            // Close Accessory
            builder.WriteStructEnd();
        }
    }
}
