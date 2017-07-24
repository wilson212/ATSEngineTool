using System.Numerics;
using Sii;

namespace ATSEngineTool.SiiEntities
{
    /// <summary>
    /// Defines an engine's characteristics for the player's vehicle, as well as some 
    /// upshift and downshift characteristics for automatic transmission gameplay. 
    /// </summary>
    /// <seealso cref="http://modding.scssoft.com/wiki/Documentation/Engine/Units/accessory_engine_data"/>
    [SiiUnit("accessory_engine_data")]
    public sealed class AccessoryEngineData : AccessoryData
    {
        /// <summary>
        /// Gets the information display info for the engine.
        /// </summary>
        /// <remarks>
        /// The first element is the rated power line.
        /// The second element is the peak torque line.
        /// The third element is the peak torque speed line.
        /// </remarks>
        /// <example>
        /// info[0]: "197 @@hp@@ (147@@kw@@)"
        /// info[1]: "550 @@lb_ft@@ (782 @@nm@@)"
        /// info[2]: "1@@dg@@200 @@rpm@@"
        /// </example>
        [SiiAttribute("info")]
        public string[] Info { get; private set; }

        /// <summary>
        /// Gets the maximum torque output of the engine in N·m.
        /// </summary>
        [SiiAttribute("torque")]
        public float Torque { get; private set; }

        /// <summary>
        /// Gets the resistive torque in N·m at 2000rpm, used for passive engine braking and consumption.
        /// </summary>
        /// <remarks>
        /// New as of ETS 1.27/ATS 1.6
        /// </remarks>
        [SiiAttribute("resistance_torque")]
        public float ResistanceTorque { get; private set; } = -1;

        /// <summary>
        /// Gets the maximum engine speed the virtual engine controller will allow except 
        /// if the engine is being driven by the wheels (e.g. if the vehicle is running away 
        /// downhill or the player makes an ill-advised downshift).
        /// </summary>
        [SiiAttribute("rpm_limit")]
        public float RpmLimit { get; private set; }

        /// <summary>
        /// Gets the (four-stroke) volumetric displacement of the engine in liters. 
        /// It is used in fuel consumption calculations.
        /// </summary>
        [SiiAttribute("volume")]
        public float Volume { get; private set; }

        /// <summary>
        /// Gets tthe Engine braking torque relative to a typical exhaust brake on an engine of the same volume.
        /// </summary>
        [SiiAttribute("engine_brake")]
        public float BrakeStrength { get; private set; } = 1.0f;

        /// <summary>
        /// When true, automatic transmissions will downshift upon beginning engine brake behavior, within rpm_range_engine_brake.
        /// </summary>
        [SiiAttribute("engine_brake_downshift")]
        public int BrakeDownshift { get; private set; } = 0;

        /// <summary>
        /// Defines the number of strength levels for the engine brake.
        /// </summary>
        /// <remarks>
        /// The braking torque at each position is based linearly on the number of positions. 
        /// So for engine_brake_positions: 3 the first position will have 1⁄3 of the maximum strength, 
        /// the second position will have 2⁄3 of the maximum strength, and the highest position will 
        /// have the maximum strength.
        /// </remarks>
        [SiiAttribute("engine_brake_positions")]
        public int BrakePositions { get; private set; } = 3;

        /// <summary>
        /// Gets the engine speed the virtual engine controller will attempt to maintain with no throttle input.
        /// </summary>
        [SiiAttribute("rpm_idle")]
        public int IdleRpm { get; private set; } = 650;

        /// <summary>
        /// Gets the maximum engine speed the virtual engine controller will allow if the transmission is in neutral – 
        /// unless rpm_limit is lower, in which case it is clamped.
        /// </summary>
        [SiiAttribute("rpm_limit_neutral")]
        public int RpmLimitNeutral { get; private set; } = 2200;

        /// <summary>
        /// Defines the torque output of the engine at various engine speeds, relative to the maximum torque defined in 
        /// <see cref="Torque"/>.
        /// </summary>
        [SiiAttribute("torque_curve")]
        public Vector2[] TorqueCurves { get; private set; }

        /// <summary>
        /// Defines the downshift and upshift engine speeds in low-range gears. (Automatic transmission)
        /// </summary>
        [SiiAttribute("rpm_range_low_gear")]
        public Vector2 RpmRange_LowGear { get; set; }

        /// <summary>
        /// Defines the downshift and upshift engine speeds in high-range gears. (Automatic transmission)
        /// </summary>
        [SiiAttribute("rpm_range_high_gear")]
        public Vector2 RpmRange_HighGear { get; set; }

        /// <summary>
        /// Defines the change to rpm_range_low_gear and rpm_range_high_gear at full throttle when adaptive 
        /// automatic transmission is enabled.
        /// </summary>
        [SiiAttribute("rpm_range_power_boost")]
        public Vector2 RpmRange_PowerBoost { get; set; }

        /// <summary>
        /// Defines the downshift and upshift engine speeds while the engine brake is operating.
        /// </summary>
        [SiiAttribute("rpm_range_engine_brake")]
        public Vector2 RpmRangeEngineBrake { get; set; } = new Vector2(0, 0);

        /// <summary>
        /// Gets the fuel consumption scale of the engine relative to the game's calculated value.
        /// </summary>
        [SiiAttribute("consumption_coef")]
        public float FuelConsumption { get; set; } = 1f;

        /// <summary>
        /// This is how many liters of AdBlue (Diesel Exhaust Fluid) are consumed per liter of fuel.
        /// </summary>
        [SiiAttribute("adblue_consumption")]
        public float AdblueConsumption { get; set; } = 0f;

        /// <summary>
        /// When the AdBlue tank is empty, the engine's power is scaled by this factor.
        /// </summary>
        [SiiAttribute("no_adblue_power_limit")]
        public float NoAdbluePowerLimit { get; set; } = 0f;
    }
}
