using System;
using CrossLite;
using CrossLite.CodeFirst;

namespace ATSEngineTool.Database
{ 
    /// <summary>
    /// Represents an <see cref="Database.Sound"/> override for a
    /// <see cref="SoundAttribute"/> on a <see cref="Database.Truck"/>
    /// </summary>
    [Table]
    public class TruckSoundOverride : IEquatable<TruckSoundOverride>
    {
        #region Columns

        /// <summary>
        /// Gets the Row ID for this <see cref="Database.Truck"/>
        /// </summary>
        [Column, PrimaryKey]
        public int TruckId { get; set; }

        /// <summary>
        /// Gets or Sets the <see cref="SoundAttribute"/> for this sound
        /// </summary>
        [Column, PrimaryKey]
        public SoundAttribute Attribute { get; set; }

        /// <summary>
        /// Gets the Row ID for this new <see cref="Database.Sound"/>
        /// </summary>
        [Column, Required]
        public int SoundPackageId { get; set; }

        #endregion

        #region Virtual Foreign Keys

        /// <summary>
        /// Gets the <see cref="Database.Truck"/> entity that this entity references.
        /// </summary>
        [InverseKey(nameof(Database.Truck.Id))]
        [ForeignKey(nameof(TruckId),
            OnDelete = ReferentialIntegrity.Cascade,
            OnUpdate = ReferentialIntegrity.Cascade
        )]
        protected virtual ForeignKey<Truck> FK_Truck { get; set; }

        /// <summary>
        /// Gets the <see cref="Database.SoundPackage"/> entity that this entity references.
        /// </summary>
        [InverseKey(nameof(Database.SoundPackage.Id))]
        [ForeignKey(nameof(SoundPackageId),
            OnDelete = ReferentialIntegrity.Cascade,
            OnUpdate = ReferentialIntegrity.Cascade
        )]
        protected virtual ForeignKey<SoundPackage> FK_Package { get; set; }

        #endregion

        #region Foreign Key Properties

        /// <summary>
        /// Gets or sets the <see cref="Database.Truck"/> that 
        /// this <see cref="TruckSoundOverride"/> is attached to.
        /// </summary>
        public Truck Truck
        {
            get
            {
                return FK_Truck?.Fetch();
            }
            set
            {
                TruckId = value.Id;
                FK_Truck?.Refresh();
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Database.SoundPackage"/> that 
        /// this <see cref="TruckSoundOverride"/> is attached to.
        /// </summary>
        public SoundPackage SoundPackage
        {
            get
            {
                return FK_Package?.Fetch();
            }
            set
            {
                SoundPackageId = value.Id;
                FK_Package?.Refresh();
            }
        }

        #endregion

        public bool Equals(TruckSoundOverride other)
        {
            if (other == null) return false;
            return (TruckId == other.TruckId && Attribute == other.Attribute);
        }

        public override bool Equals(object obj) => Equals(obj as TruckSoundOverride);

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
