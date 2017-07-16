using System;
using System.Collections.Generic;
using CrossLite;
using CrossLite.CodeFirst;

namespace ATSEngineTool.Database
{
    [Table]
    public class Truck : IEquatable<Truck>
    {
        /// <summary>
        /// The Truck Id
        /// </summary>
        [Column, PrimaryKey, AutoIncrement]
        public int Id { get; protected set; }

        /// <summary>
        /// Gets or Sets the SII Unitname of this truck
        /// </summary>
        [Column, Required, Unique, Collation(Collation.NoCase)]
        public string UnitName { get; set; }

        /// <summary>
        /// Gets or Sets the Name of this truck
        /// </summary>
        [Column, Required, Unique]
        public string Name { get; set; }

        /// <summary>
        /// Gets or Sets the <see cref="TruckSoundPackage"/> used by this truck
        /// </summary>
        [Column, Required]
        public int SoundPackageId { get; set; }

        /// <summary>
        /// Gets or Sets whether this is an SCS truck, or a Modded truck
        /// </summary>
        [Column, Required, Default(false)]
        public bool IsScsTruck { get; set; } = false;

        #region Virtual Foreign Keys

        /// <summary>
        /// Gets the <see cref="TruckSoundPackage"/> entity that this entity references.
        /// </summary>
        [InverseKey(nameof(Database.SoundPackage.Id))]
        [ForeignKey(nameof(SoundPackageId),
            OnDelete = ReferentialIntegrity.Restrict,
            OnUpdate = ReferentialIntegrity.Cascade
        )]
        protected virtual ForeignKey<TruckSoundPackage> FK_SoundPack { get; set; }

        #endregion

        #region Foreign Key Properties

        /// <summary>
        /// Gets or sets the <see cref="Database.Truck"/> that 
        /// this <see cref="TruckSoundOverride"/> is attached to.
        /// </summary>
        public TruckSoundPackage SoundPackage
        {
            get
            {
                return FK_SoundPack?.Fetch();
            }
            set
            {
                SoundPackageId = value.Id;
                FK_SoundPack?.Refresh();
            }
        }

        #endregion

        #region Child Database Sets

        /// <summary>
        /// Gets a list of <see cref="TruckSoundSetting"/> entities that reference this 
        /// <see cref="Truck"/>
        /// </summary>
        /// <remarks>
        /// A lazy loaded enumeration that fetches all TruckEngine objects
        /// that are bound by the foreign key and this Truck.Id.
        /// </remarks>
        public virtual IEnumerable<TruckSoundSetting> SoundSetting { get; set; }

        /// <summary>
        /// Gets a list of <see cref="Engine"/> entities that reference this 
        /// <see cref="Truck"/>
        /// </summary>
        /// <remarks>
        /// A lazy loaded enumeration that fetches all TruckEngine objects
        /// that are bound by the foreign key and this Truck.Id.
        /// </remarks>
        public virtual IEnumerable<TruckEngine> TruckEngines { get; set; }

        /// <summary>
        /// Gets a list of <see cref="Transmission"/> entities that reference this 
        /// <see cref="Trauck"/>
        /// </summary>
        /// <remarks>
        /// A lazy loaded enumeration that fetches all TruckTransmission objects
        /// that are bound by the foreign key and this Truck.Id.
        /// </remarks>
        public virtual IEnumerable<TruckTransmission> TruckTransmissions { get; set; }

        #endregion

        /// <summary>
        /// Compares a <see cref="Truck"/> with this one, and returns whether
        /// or not the Id and UnitNames match
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsDuplicateOf(Truck other)
        {
            return (Id == other.Id && UnitName.Equals(other.UnitName, StringComparison.InvariantCultureIgnoreCase));
        }

        public bool Equals(Truck other)
        {
            if (other == null) return false;
            return (Id == other.Id);
        }

        public override bool Equals(object obj) => Equals(obj as Truck);

        public override int GetHashCode() => Id.GetHashCode();

        public override string ToString() => Name;
    }
}
