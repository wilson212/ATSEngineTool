using System.Linq;
using CrossLite;
using CrossLite.CodeFirst;

namespace ATSEngineTool.Database
{
    [Table]
    [CompositeUnique("PackageId", "FileName")]
    public class EngineSound
    {
        #region Column Attributes

        /// <summary>
        /// Gets the Row ID for this <see cref="EngineSound"/>
        /// </summary>
        [Column, PrimaryKey, AutoIncrement]
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
        public string FileName { get; private set; }

        /// <summary>
        /// Gets or Sets whether this sound is looped in game, or played just once
        /// </summary>
        [Column, Required, Default(false)]
        public bool Looped { get; private set; }

        /// <summary>
        /// Gets or Sets whether this <see cref="EngineSound"/>'s volume changes
        /// based on the players position relevent to the sounds source.
        /// </summary>
        [Column, Required, Default(false)]
        public bool Is2D { get; private set; }

        /// <summary>
        /// Gets or Sets the sounds volume in game
        /// </summary>
        [Column, Required]
        public double Volume { get; private set; }

        /// <summary>
        /// Gets or Sets the Engine RPM's pitch reference to this sound. The
        /// greater difference between RPM and this volume, the greater the 
        /// pitch of this sound is adjusted.
        /// </summary>
        [Column, Required, Default(0)]
        public double PitchReference { get; private set; } = 0;

        /// <summary>
        /// Gets or Sets the minimum engine RPM to play this sound. Set to
        /// zero to ignore minimum engine RPM
        /// </summary>
        [Column, Required, Default(0)]
        public double MinRpm { get; private set; } = 0;

        /// <summary>
        /// Gets or Sets the maximum engine RPM to play this sound. Set to
        /// zero to ignore maximum engine RPM
        /// </summary>
        [Column, Required, Default(0)]
        public double MaxRpm { get; private set; } = 0;

        #endregion

        #region Foreign Keys

        [InverseKey("Id")]
        [ForeignKey("PackageId", OnDelete = ReferentialIntegrity.Cascade)]
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

        private static SoundAttribute[] EngineSounds = new[]
        {
            SoundAttribute.Engine,
            SoundAttribute.EngineBrake,
            SoundAttribute.EngineLoad,
            SoundAttribute.EngineNoFuel,
            SoundAttribute.Start,
            SoundAttribute.StartNoFuel,
            SoundAttribute.Stop
        };

        /// <summary>
        /// Gets whether this sound is an engine sound, or truck sound.
        /// </summary>
        public bool IsEngineSound => EngineSounds.Contains(this.Attribute);

        #endregion
    }
}
