using Sii;

namespace ATSEngineTool.SiiEntities
{
    [SiiUnit("accessory_sound_data")]
    public class AccessorySoundData
    {
        [SiiAttribute("name")]
        public string Name { get; private set; }

        [SiiAttribute("start")]
        public SoundData Start { get; private set; }

        [SiiAttribute("stop")]
        public SoundData Stop { get; private set; }

        [SiiAttribute("start_no_fuel")]
        public SoundData StartNoFuel { get; private set; }

        [SiiAttribute("engine")]
        public SoundEngineData[] Engine { get; private set; }

        [SiiAttribute("engine_load")]
        public SoundEngineData[] EngineLoad { get; private set; }

        [SiiAttribute("engine_nofuel")]
        public SoundEngineData[] EngineNoFuel { get; private set; }

        [SiiAttribute("turbo")]
        public SoundData Turbo { get; private set; }

        [SiiAttribute("air_gear")]
        public SoundData[] AirGears { get; private set; }

        [SiiAttribute("air_brake")]
        public SoundData[] AirBrakes { get; private set; }

        [SiiAttribute("engine_exhaust")]
        public SoundEngineData[] EngineExhaust{ get; private set; }

        [SiiAttribute("engine_brake")]
        public SoundEngineData[] EngineBrake { get; private set; }

        [SiiAttribute("horn")]
        public SoundData Horn { get; private set; }

        [SiiAttribute("air_horn")]
        public SoundData AirHorn { get; private set; }

        [SiiAttribute("reverse")]
        public SoundData Reverse { get; private set; }

        [SiiAttribute("change_gear")]
        public SoundData ChangeGear { get; private set; }

        [SiiAttribute("blinker_on")]
        public SoundData BlinkerOn { get; private set; }

        [SiiAttribute("blinker_off")]
        public SoundData BlinkerOff { get; private set; }

        [SiiAttribute("wipers_up")]
        public SoundData WipersUp { get; private set; }

        [SiiAttribute("wipers_down")]
        public SoundData WipersDown { get; private set; }

        [SiiAttribute("exterior_sound")]
        public bool Exterior { get; private set; }

        [SiiAttribute("suitable_for")]
        public string[] SuitableEngines { get; private set; }
    }
}
