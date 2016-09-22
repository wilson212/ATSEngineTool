using System;
using System.Collections.Generic;
using CrossLite;
using CrossLite.CodeFirst;

namespace ATSEngineTool.Database
{
    [Table]
    [CompositeUnique("Manufacturer", "Name")]
    public class EngineSeries
    {
        /// <summary>
        /// Gets or Sets the Unique ID for this entity
        /// </summary>
        [Column, PrimaryKey, AutoIncrement]
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
        [Column, Required, Default(12.9)]
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
        public int SoundId { get; set; } = 1;

        /// <summary>
        /// Gets or Sets the <see cref="Database.SoundPackage"/> package bound to 
        /// this engine
        /// </summary>
        public SoundPackage SoundPackage
        {
            get
            {
                return FK_EngineSound?.Fetch();
            }
            set
            {
                if (value == null)
                    throw new Exception("Engine Sound cannot be NULL");

                SoundId = value.Id;
                FK_EngineSound?.Refresh();
            }
        }

        /// <summary>
        /// Gets a list of <see cref="Engine"/> entities that reference this 
        /// <see cref="EngineSeries"/>
        /// </summary>
        public virtual IEnumerable<Engine> Engines { get; set; }

        #region Virtual Foreign Keys

        [InverseKey("Id")]
        [ForeignKey("SoundId", OnDelete = ReferentialIntegrity.Cascade)]
        protected virtual ForeignKey<SoundPackage> FK_EngineSound { get; set; }

        #endregion

        public override string ToString() => $"{Manufacturer} {Name}";

        public override bool Equals(object obj)
        {
            if (obj is EngineSeries)
            {
                EngineSeries compare = (EngineSeries)obj;
                return compare.Id == this.Id;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }
    }
}
