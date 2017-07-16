using System.Collections.Generic;
using CrossLite;
using CrossLite.CodeFirst;

namespace ATSEngineTool.Database
{
    [Table]
    public class TransmissionSeries
    {
        /// <summary>
        /// Gets or sets the unique row id for this object
        /// </summary>
        [Column, PrimaryKey]
        public int Id { get; set; }

        /// <summary>
        /// The Unique brand name
        /// </summary>
        [Column, Required, Unique, Collation(Collation.NoCase)]
        public string Name { get; set; }

        /// <summary>
        /// The Unique brand name
        /// </summary>
        [Column, Required, Default("transmission_generic")]
        public string Icon { get; set; } = "transmission_generic";

        /// <summary>
        /// Gets a list of <see cref="Transmissions"/> entities that reference this 
        /// <see cref="TransmissionSeries"/>
        /// </summary>
        /// <remarks>
        /// A lazy loaded enumeration that fetches all Transmissions
        /// that are bound by the foreign key and this TransmissionSeries.Id.
        /// </remarks>
        public virtual IEnumerable<Transmission> Transmissions { get; set; }

        public override string ToString() => Name;
    }
}
