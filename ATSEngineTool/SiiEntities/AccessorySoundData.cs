using ATSEngineTool.Database;
using Sii;

namespace ATSEngineTool.SiiEntities
{
    [SiiUnit("accessory_sound_data")]
    public class AccessorySoundData
    {
        [SiiAttribute("name")]
        public string Name { get; private set; }

        [SiiAttribute("start"), SoundAttribute(SoundAttribute.Start)]
        public SoundData Start { get; private set; }

        [SiiAttribute("stop"), SoundAttribute(SoundAttribute.Stop)]
        public SoundData Stop { get; private set; }

        [SiiAttribute("start_no_fuel"), SoundAttribute(SoundAttribute.StartNoFuel)]
        public SoundData StartNoFuel { get; private set; }

        [SiiAttribute("engine"), SoundAttribute(SoundAttribute.Engine)]
        public SoundEngineData[] Engine { get; private set; }

        [SiiAttribute("engine_load"), SoundAttribute(SoundAttribute.EngineLoad)]
        public SoundEngineData[] EngineLoad { get; private set; }

        [SiiAttribute("engine_nofuel"), SoundAttribute(SoundAttribute.EngineNoFuel)]
        public SoundEngineData[] EngineNoFuel { get; private set; }

        [SiiAttribute("turbo"), SoundAttribute(SoundAttribute.Turbo)]
        public SoundData Turbo { get; private set; }

        [SiiAttribute("air_gear"), SoundAttribute(SoundAttribute.AirGears)]
        public SoundData[] AirGears { get; private set; }

        [SiiAttribute("air_brake"), SoundAttribute(SoundAttribute.AirBrakes)]
        public SoundData[] AirBrakes { get; private set; }

        [SiiAttribute("engine_exhaust"), SoundAttribute(SoundAttribute.EngineExhaust)]
        public SoundEngineData[] EngineExhaust{ get; private set; }

        [SiiAttribute("engine_brake"), SoundAttribute(SoundAttribute.EngineBrake)]
        public SoundEngineData[] EngineBrake { get; private set; }

        [SiiAttribute("horn"), SoundAttribute(SoundAttribute.Horn)]
        public SoundData Horn { get; private set; }

        [SiiAttribute("air_horn"), SoundAttribute(SoundAttribute.AirHorn)]
        public SoundData AirHorn { get; private set; }

        [SiiAttribute("reverse"), SoundAttribute(SoundAttribute.Reverse)]
        public SoundData Reverse { get; private set; }

        [SiiAttribute("change_gear"), SoundAttribute(SoundAttribute.ChangeGear)]
        public SoundData ChangeGear { get; private set; }

        [SiiAttribute("blinker_on"), SoundAttribute(SoundAttribute.BlinkerOn)]
        public SoundData BlinkerOn { get; private set; }

        [SiiAttribute("blinker_off"), SoundAttribute(SoundAttribute.BlinkerOff)]
        public SoundData BlinkerOff { get; private set; }

        [SiiAttribute("wipers_up"), SoundAttribute(SoundAttribute.WiperUp)]
        public SoundData WipersUp { get; private set; }

        [SiiAttribute("wipers_down"), SoundAttribute(SoundAttribute.WiperDown)]
        public SoundData WipersDown { get; private set; }

        [SiiAttribute("exterior_sound")]
        public bool Exterior { get; private set; }

        [SiiAttribute("suitable_for")]
        public string[] SuitableEngines { get; private set; }
    }
}
