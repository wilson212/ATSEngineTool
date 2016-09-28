using CrossLite;
using CrossLite.CodeFirst;

namespace ATSEngineTool.Database
{
    [Table]
    public class TransmissionGear
    {
        [Column, PrimaryKey]
        public int TransmissionId { get; set; }

        [Column, PrimaryKey]
        public int GearIndex { get; set; }

        public bool IsReverse => (Ratio < 0m);

        /// <summary>
        /// Gets or Sets the string name of this gear
        /// </summary>
        [Column, Required, Default("")]
        public string Name { get; set; } = string.Empty;

        [Column, Required]
        public decimal Ratio { get; set; }

        #region Foreign Keys

        [InverseKey("Id")]
        [ForeignKey("TransmissionId", 
            OnDelete = ReferentialIntegrity.Cascade,
            OnUpdate = ReferentialIntegrity.Cascade
        )]
        protected virtual ForeignKey<Transmission> FK_Transmission { get; set; }

        #endregion

        /// <summary>
        /// Gets or Sets the <see cref="ATSEngineTool.Database.Transmission"/> that 
        /// this gear relates to.
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
    }
}
