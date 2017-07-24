using Sii;

namespace ATSEngineTool.SiiEntities
{
    /// <summary>
    /// Define custom names for the gears of a transmission which are displayed in the UI and dashboard.
    /// </summary>
    /// <seealso cref="http://modding.scssoft.com/wiki/Documentation/Engine/Units/transmission_names"/>
    [SiiUnit("transmission_names")]
    public class TransmissionNames
    {
        /// <summary>
        /// Gets the gear names to be displayed for forward gears. 
        /// </summary>
        [SiiAttribute("forward")]
        public string[] Forward { get; private set; }

        /// <summary>
        /// Gets the gear names to be displayed for reverse gears. 
        /// </summary>
        [SiiAttribute("reverse")]
        public string[] Reverse { get; private set; }

        /// <summary>
        /// Gets the gear name to be displayed when the transmission is in neutral.
        /// </summary>
        [SiiAttribute("neutral")]
        public string Neutral { get; private set; }
    }
}
