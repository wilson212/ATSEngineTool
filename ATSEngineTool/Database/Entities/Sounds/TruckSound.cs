using ATSEngineTool.SiiEntities;
using CrossLite;
using CrossLite.CodeFirst;

namespace ATSEngineTool.Database
{
    [Table]
    public class TruckSound : Sound
    {
        #region Foreign Keys

        [InverseKey(nameof(Database.SoundPackage.Id))]
        [ForeignKey(nameof(PackageId),
            OnDelete = ReferentialIntegrity.Cascade,
            OnUpdate = ReferentialIntegrity.Cascade
        )]
        protected virtual ForeignKey<TruckSoundPackage> FK_Package { get; set; }

        /// <summary>
        /// Gets or Sets the <see cref="ATSEngineTool.Database.SoundPackage"/> that 
        /// this Sound references.
        /// </summary>
        public TruckSoundPackage Package
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

        public override SoundType SoundType => SoundType.Truck;

        /// <summary>
        /// Creates a new instance of <see cref="TruckSound"/>
        /// </summary>
        public TruckSound() { }

        /// <summary>
        /// Creates a new instance of <see cref="TruckSound"/>
        /// </summary>
        public TruckSound(SoundData data, SoundAttribute attr, SoundLocation location)
        {
            this.Location = location;
            this.Attribute = attr;

            this.FileName = data.Name;
            this.Is2D = data.Is2D;
            this.Looped = data.Looped;
            this.Volume = data.Volume;
        }

        /// <summary>
        /// Appends this <see cref="TruckSound"/> to an open <see cref="SiiFileBuilder"/> object
        /// </summary>
        public override void AppendTo(SiiFileBuilder builder, string objectName, SoundPackage package)
        {
            // Begin the accessory
            builder.WriteStructStart("sound_data", objectName);

            // Write attributes
            builder.WriteAttribute("name", GetParsedFileNamePath(package));
            builder.WriteAttribute("looped", this.Looped);

            if (this.Is2D)
                builder.WriteAttribute("is_2d", true);

            if (this.Volume != 1.0)
                builder.WriteAttribute("volume", this.Volume);

            // Close Accessory
            builder.WriteStructEnd();
        }
    }
}
