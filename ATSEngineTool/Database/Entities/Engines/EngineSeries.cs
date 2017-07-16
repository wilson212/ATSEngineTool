using System;
using System.Collections.Generic;
using CrossLite;
using CrossLite.CodeFirst;

namespace ATSEngineTool.Database
{
    [Table]
    [CompositeUnique(nameof(Manufacturer), nameof(Name))]
    public class EngineSeries : IEquatable<EngineSeries>
    {
        /// <summary>
        /// Gets or Sets the Unique ID for this entity
        /// </summary>
        [Column, PrimaryKey]
        public int Id { get; set; }

        /// <summary>
        /// The Unique brand name
        /// </summary>
        [Column, Required, Collation(Collation.NoCase)]
        public string Manufacturer { get; set; }

        /// <summary>
        /// The Unique brand name
        /// </summary>
        [Column, Required, Collation(Collation.NoCase)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or Sets the Displacement (in liters)
        /// </summary>
        [Column, Required, Default("12.9", Quote = false)]
        public decimal Displacement { get; set; } = 12.9m;

        /// <summary>
        /// The Unique brand name
        /// </summary>
        [Column, Required, Default("engine_01")]
        public string EngineIcon { get; set; } = "engine_01";

        /// <summary>
        /// The Unique brand name
        /// </summary>
        [Column, Required]
        public int SoundPackageId { get; set; } = 1;

        /// <summary>
        /// Gets or Sets the <see cref="Database.EngineSoundPackage"/> package bound to 
        /// this series of engines
        /// </summary>
        public EngineSoundPackage SoundPackage
        {
            get
            {
                return FK_EngineSound?.Fetch();
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("Engine Sound Package cannot be NULL");

                SoundPackageId = value.Id;
                FK_EngineSound?.Refresh();
            }
        }

        /// <summary>
        /// Gets a list of <see cref="Engine"/> entities that reference this 
        /// <see cref="EngineSeries"/>
        /// </summary>
        /// <remarks>
        /// A lazy loaded enumeration that fetches all Engines
        /// that are bound by the foreign key and this EngineSeries.Id.
        /// </remarks>
        public virtual IEnumerable<Engine> Engines { get; set; }

        #region Virtual Foreign Keys

        [InverseKey(nameof(Database.SoundPackage.Id))]
        [ForeignKey(nameof(SoundPackageId),
            OnDelete = ReferentialIntegrity.Cascade,
            OnUpdate = ReferentialIntegrity.Cascade
        )]
        protected virtual ForeignKey<EngineSoundPackage> FK_EngineSound { get; set; }

        #endregion

        public bool Equals(EngineSeries other)
        {
            if (other == null) return false;
            return other.Id == Id;
        }

        public override string ToString() => $"{Manufacturer} {Name}";

        public override bool Equals(object obj) => Equals(obj as EngineSeries);

        public override int GetHashCode() => Id.GetHashCode();
    }
}
