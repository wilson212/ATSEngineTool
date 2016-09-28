using CrossLite;
using CrossLite.CodeFirst;

namespace ATSEngineTool.Database
{
    [Table]
    public class TruckTransmission
    {
        /// <summary>
        /// The Unique item ID
        /// </summary>
        [Column, Required, PrimaryKey]
        protected int TransmissionId { get; set; }

        /// <summary>
        /// The Unique item ID
        /// </summary>
        [Column, Required, PrimaryKey]
        protected int TruckId { get; set; }

        #region Virtual Foreign Keys

        [InverseKey("Id")]
        [ForeignKey("TruckId", 
            OnDelete = ReferentialIntegrity.Cascade,
            OnUpdate = ReferentialIntegrity.Cascade
        )]
        protected virtual ForeignKey<Truck> FK_Truck { get; set; }

        [InverseKey("Id")]
        [ForeignKey("TransmissionId",
            OnDelete = ReferentialIntegrity.Cascade,
            OnUpdate = ReferentialIntegrity.Cascade
        )]
        protected virtual ForeignKey<Transmission> FK_Transmission { get; set; }

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
        public Transmission Transmission
        {
            get
            {
                return FK_Transmission?.Fetch();
            }
            set
            {
                TransmissionId = value.Id;
                FK_Transmission?.Refresh();
            }
        }

        #endregion
    }
}
