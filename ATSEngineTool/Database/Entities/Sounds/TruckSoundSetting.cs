using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrossLite;
using CrossLite.CodeFirst;

namespace ATSEngineTool.Database
{
    /// <summary>
    /// Represents a 1:0 or 1:1 relationship between a <see cref="Truck"/> and
    /// <see cref="Database.EngineSoundPackage"/>
    /// </summary>
    [Table]
    public class TruckSoundSetting : IEquatable<TruckSoundSetting>
    {
        #region Columns

        /// <summary>
        /// The Truck Id
        /// </summary>
        [Column, PrimaryKey]
        public int TruckId { get; set; }

        /// <summary>
        /// Gets or Sets the Default <see cref="Database.EngineSoundPackage"/> ID for this truck
        /// </summary>
        [Column, Required]
        public int EngineSoundPackageId { get; set; }

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
        /// Gets the <see cref="Database.EngineSoundPackage"/> entity that this entity references.
        /// </summary>
        [InverseKey(nameof(Database.EngineSoundPackage.Id))]
        [ForeignKey(nameof(EngineSoundPackageId),
            OnDelete = ReferentialIntegrity.Cascade,
            OnUpdate = ReferentialIntegrity.Cascade
        )]
        protected virtual ForeignKey<EngineSoundPackage> FK_ESP { get; set; }

        #endregion

        #region Foreign Key Properties

        /// <summary>
        /// Gets or sets the default <see cref="Database.Truck"/> that 
        /// this <see cref="TruckSoundSetting"/> references.
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
        /// Gets or sets the default <see cref="Database.EngineSoundPackage"/> that 
        /// this <see cref="TruckSoundSetting"/> references.
        /// </summary>
        public EngineSoundPackage EngineSoundPackage
        {
            get
            {
                return FK_ESP?.Fetch();
            }
            set
            {
                EngineSoundPackageId = value.Id;
                FK_ESP?.Refresh();
            }
        }

        /// <summary>
        /// Compares a <see cref="TruckSoundSetting"/> with this one, and returns whether
        /// or not the attributes match
        /// </summary>
        /// <remarks>Used in the <see cref="TruckEditForm"/></remarks>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool IsDuplicateOf(TruckSoundSetting other)
        {
            return (
                TruckId == other.TruckId &&
                EngineSoundPackageId == other.EngineSoundPackageId
            );
        }

        public bool Equals(TruckSoundSetting other)
        {
            if (other == null) return false;
            return other.TruckId == TruckId;
        }

        public override bool Equals(object obj) => Equals(obj as TruckSoundSetting);

        public override int GetHashCode() => TruckId.GetHashCode();

        #endregion


    }
}
