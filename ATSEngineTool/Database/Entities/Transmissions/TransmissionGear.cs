using CrossLite;
using CrossLite.CodeFirst;

namespace ATSEngineTool.Database
{
    [Table]
    public class TransmissionGear
    {
        /// <summary>
        /// Gets or sets the parent <see cref="Transmission.Id"/>
        /// </summary>
        [Column, PrimaryKey]
        public int TransmissionId { get; set; }

        /// <summary>
        /// Gets or sets the index within the <see cref="Transmission"/>
        /// </summary>
        [Column, PrimaryKey]
        public int GearIndex { get; set; }

        /// <summary>
        /// Gets whether this gear is a reverse gear
        /// </summary>
        public bool IsReverse => (Ratio < 0m);

        /// <summary>
        /// Gets or Sets the string name of this gear
        /// </summary>
        [Column, Required, Default("")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the ratio of this gear
        /// </summary>
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
