using System.Numerics;
using Sii;

namespace ATSEngineTool
{
    [SiiUnit("accessory_engine_data")]
    internal sealed class AccessoryEngineData
    {
        [SiiAttribute("name")]
        public string Name { get; private set; }

        [SiiAttribute("price")]
        public ushort Price { get; private set; }

        [SiiAttribute("unlock")]
        public byte UnlockLevel { get; private set; }

        [SiiAttribute("info")]
        public string[] Info { get; private set; }

        [SiiAttribute("icon")]
        public string Icon { get; private set; }

        [SiiAttribute("torque")]
        public uint Torque { get; private set; }

        [SiiAttribute("rpm_limit")]
        public uint RpmLimit { get; private set; }

        [SiiAttribute("volume")]
        public float Volume { get; private set; }

        [SiiAttribute("engine_brake")]
        public float BrakeStrength { get; private set; } = 1.0f;

        [SiiAttribute("engine_brake_downshift")]
        public int BrakeDownshift { get; private set; } = 0;

        [SiiAttribute("engine_brake_positions")]
        public int BrakePositions { get; private set; } = 3;

        [SiiAttribute("rpm_idle")]
        public int IdleRpm { get; private set; } = 650;

        [SiiAttribute("rpm_limit_neutral")]
        public int RpmLimitNeutral { get; private set; } = 2200;

        [SiiAttribute("defaults")]
        public string[] Defaults { get; private set; }

        [SiiAttribute("conflict_with")]
        public string[] Conflicts { get; private set; }

        [SiiAttribute("torque_curve")]
        public Vector2[] TorqueCurve { get; private set; }

        [SiiAttribute("rpm_range_low_gear")]
        public Vector2 RpmRange_LowGear { get; set; }

        [SiiAttribute("rpm_range_high_gear")]
        public Vector2 RpmRange_HighGear { get; set; }

        [SiiAttribute("rpm_range_power_boost")]
        public Vector2 RpmRange_PowerBoost { get; set; }

        [SiiAttribute("rpm_range_engine_brake")]
        public Vector2 RpmRangeEngineBrake { get; set; } = new Vector2(0, 0);

        [SiiAttribute("adblue_consumption")]
        public float AdblueConsumption { get; set; } = 0f;

        [SiiAttribute("no_adblue_power_limit")]
        public float NoAdbluePowerLimit { get; set; } = 0f;
    }
}
