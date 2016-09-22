using CrossLite;
using CrossLite.CodeFirst;

namespace ATSEngineTool.Database
{
    [Table]
    public class TruckEngine
    {
        #region Column Properties

        /// <summary>
        /// The Unique item ID
        /// </summary>
        [Column, Required, PrimaryKey]
        protected int TruckId { get; set; }

        /// <summary>
        /// The Unique item ID
        /// </summary>
        [Column, Required, PrimaryKey]
        protected int EngineId { get; set; }

        #endregion

        #region Virtual Foreign Keys

        [InverseKey("Id")]
        [ForeignKey("TruckId", OnDelete = ReferentialIntegrity.Cascade)]
        protected virtual ForeignKey<Truck> FK_Truck { get; set; }

        [InverseKey("Id")]
        [ForeignKey("EngineId", OnDelete = ReferentialIntegrity.Cascade)]
        protected virtual ForeignKey<Engine> FK_Engine { get; set; }

        #endregion

        #region Foreign Key Properties

        /// <summary>
        /// Gets or Sets the <see cref="ATSEngineTool.Database.EngineList"/> that 
        /// this truck will use in game.
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
        /// Gets or Sets the <see cref="ATSEngineTool.Database.EngineList"/> that 
        /// this truck will use in game.
        /// </summary>
        public Engine Engine
        {
            get
            {
                return FK_Engine?.Fetch();
            }
            set
            {
                EngineId = value.Id;
                FK_Engine?.Refresh();
            }
        }

        #endregion
    }
}
