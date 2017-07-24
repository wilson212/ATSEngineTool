using Sii;

namespace ATSEngineTool.SiiEntities
{
    /// <summary>
    /// The <see cref="AccessoryTransmissionData"/> class is used to define the transmission characteristics 
    /// for the player's vehicle, as well as the differential ratio.
    /// </summary>
    /// <seealso cref="http://modding.scssoft.com/wiki/Documentation/Engine/Units/accessory_transmission_data"/>
    [SiiUnit("accessory_transmission_data")]
    public class AccessoryTransmissionData : AccessoryData
    {
        /// <summary>
        /// Gets the number of output levels for the retarder if applicable. 
        /// If zero, retarder behavior is disabled for the transmission.
        /// </summary>
        [SiiAttribute("retarder")]
        public uint Retarder { get; private set; }

        /// <summary>
        /// Gets the number of ratios_forward to handle as crawler gears for UI display.
        /// </summary>
        /// <remarks>Added in ETS2 1.27.2</remarks>
        [SiiAttribute("crawls")]
        public uint Crawls { get; private set; }

        /// <summary>
        /// Gets the differential/final drive ratio between the transmission output shaft and the wheels.
        /// </summary>
        [SiiAttribute("differential_ratio")]
        public decimal DifferentialRatio { get; private set; }

        /// <summary>
        /// Gets the Torque ratio multiplier if torque converter/multiplier is present. 
        /// Any positive value is used as multiplier on lower RPM torque.
        /// </summary>
        [SiiAttribute("stall_torque_ratio")]
        public decimal StallTorqueRatio { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [SiiAttribute("transmission_names")]
        public TransmissionNames GearNames { get; private set; }

        /// <summary>
        /// Gets the forward gear ratios in order of decreasing gear reduction. 
        /// The values should be positive.
        /// </summary>
        [SiiAttribute("ratios_forward")]
        public decimal[] ForwardRatios { get; private set; }

        /// <summary>
        /// Gets the reverse gear ratios in order of decreasing gear reduction. 
        /// The values should be negative.
        /// </summary>
        [SiiAttribute("ratios_reverse")]
        public decimal[] ReverseRatios { get; private set; }
    }
}
