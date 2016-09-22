using CrossLite;
using CrossLite.CodeFirst;

namespace ATSEngineTool.Database
{
    [Table]
    public class TorqueRatio
    {
        /// <summary>
        /// The Engine ID
        /// </summary>
        [Column, Required, PrimaryKey]
        public int EngineId { get; set; }

        /// <summary>
        /// Gets or Sets the RPM level for this torque ratio
        /// </summary>
        [Column, Required, PrimaryKey]
        public int RpmLevel { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [Column, Required]
        public decimal Ratio { get; set; }

        #region Foreign Keys

        [InverseKey("Id")]
        [ForeignKey("EngineId", OnDelete = ReferentialIntegrity.Cascade)]
        protected virtual ForeignKey<Engine> FK_Engine { get; set; }

        /// <summary>
        /// Gets or Sets the exterior <see cref="ATSEngineTool.Database.Engine"/> 
        /// object bound to this engine
        /// </summary>
        public Engine Engine
        {
            get
            {
                return FK_Engine?.Fetch();
            }
        }

        #endregion
    }
}
