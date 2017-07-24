using ATSEngineTool.Database;
using Sii;

namespace ATSEngineTool.SiiEntities
{
    /// <summary>
    /// Define sounds for the player's vehicle.
    /// </summary>
    /// <seealso cref="http://modding.scssoft.com/wiki/Documentation/Engine/Units/accessory_sound_data"/>
    [SiiUnit("accessory_sound_data")]
    public class AccessorySoundData
    {
        /// <summary>
        /// Gets the name of this <see cref="AccessorySoundData"/>
        /// </summary>
        [SiiAttribute("name")]
        public string Name { get; private set; }

        /// <summary>
        /// Points to a <see cref="SoundData"/> unit which defines the sound to play when the engine is starting.
        /// </summary>
        [SiiAttribute("start"), SoundAttribute(SoundAttribute.Start)]
        public SoundData Start { get; private set; }

        /// <summary>
        /// Points to a <see cref="SoundData"/> unit which defines the sound to play when the engine is stopped.
        /// </summary>
        [SiiAttribute("stop"), SoundAttribute(SoundAttribute.Stop)]
        public SoundData Stop { get; private set; }

        /// <summary>
        /// Points to a <see cref="SoundData"/> unit which defines the sound to play when the player 
        /// attempts to start the engine with no fuel.
        /// </summary>
        [SiiAttribute("start_no_fuel"), SoundAttribute(SoundAttribute.StartNoFuel)]
        public SoundData StartNoFuel { get; private set; }

        /// <summary>
        /// Each array member points to a <see cref="SoundEngineData"/> unit which collectively 
        /// define the engine's sound with no load (e.g. idle, cruising, gentle acceleration, etc).
        /// </summary>
        [SiiAttribute("engine"), SoundAttribute(SoundAttribute.Engine)]
        public SoundEngineData[] Engine { get; private set; }

        /// <summary>
        /// Each array member points to a <see cref="SoundEngineData"/> unit which collectively 
        /// define the engine's sound under load (e.g. heavy acceleration, climbing a hill, etc).
        /// </summary>
        [SiiAttribute("engine_load"), SoundAttribute(SoundAttribute.EngineLoad)]
        public SoundEngineData[] EngineLoad { get; private set; }

        /// <summary>
        /// Each array member points to a <see cref="SoundEngineData"/> unit which collectively 
        /// define the engine's sound when fuel has been cut (e.g. coasting in gear, speed limiter, etc).
        /// </summary>
        [SiiAttribute("engine_nofuel"), SoundAttribute(SoundAttribute.EngineNoFuel)]
        public SoundEngineData[] EngineNoFuel { get; private set; }

        /// <summary>
        /// Points to a <see cref="SoundData"/> unit which defines the sound of the turbocharger.
        /// </summary>
        [SiiAttribute("turbo"), SoundAttribute(SoundAttribute.Turbo)]
        public SoundData Turbo { get; private set; }

        /// <summary>
        /// Each member points to a <see cref="SoundData"/> unit defining one of the gear change sounds. 
        /// Sounds are played randomly during gear change.
        /// </summary>
        [SiiAttribute("air_gear"), SoundAttribute(SoundAttribute.AirGears)]
        public SoundData[] AirGears { get; private set; }

        /// <summary>
        /// Each member points to a <see cref="SoundData"/> unit defining one of the air brake sounds. 
        /// Sounds are played randomly during gear change.
        /// </summary>
        [SiiAttribute("air_brake"), SoundAttribute(SoundAttribute.AirBrakes)]
        public SoundData[] AirBrakes { get; private set; }

        /// <summary>
        /// Each array member points to a <see cref="SoundEngineData"/> unit which collectively 
        /// define the resonant sound of the truck's exhaust.
        /// </summary>
        [SiiAttribute("engine_exhaust"), SoundAttribute(SoundAttribute.EngineExhaust)]
        public SoundEngineData[] EngineExhaust{ get; private set; }

        /// <summary>
        /// Each array member points to a <see cref="SoundEngineData"/> unit which collectively 
        /// define the engine's sound while using the engine brake.
        /// </summary>
        [SiiAttribute("engine_brake"), SoundAttribute(SoundAttribute.EngineBrake)]
        public SoundEngineData[] EngineBrake { get; private set; }

        /// <summary>
        /// Points to a <see cref="SoundData"/> unit defining the sound for the truck's electric/city horn.
        /// </summary>
        [SiiAttribute("horn"), SoundAttribute(SoundAttribute.Horn)]
        public SoundData Horn { get; private set; }

        /// <summary>
        /// Points to a <see cref="SoundData"/> unit defining the default sound for the truck's air horn.
        /// </summary>
        [SiiAttribute("air_horn"), SoundAttribute(SoundAttribute.AirHorn)]
        public SoundData AirHorn { get; private set; }

        /// <summary>
        /// Points to a <see cref="SoundData"/> unit defining the sound for the truck's reverse beeper.
        /// </summary>
        [SiiAttribute("reverse"), SoundAttribute(SoundAttribute.Reverse)]
        public SoundData Reverse { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        [SiiAttribute("change_gear"), SoundAttribute(SoundAttribute.ChangeGear)]
        public SoundData ChangeGear { get; private set; }

        /// <summary>
        /// Points to a <see cref="SoundData"/> unit defining the sound of the blinker relay closing.
        /// </summary>
        [SiiAttribute("blinker_on"), SoundAttribute(SoundAttribute.BlinkerOn)]
        public SoundData BlinkerOn { get; private set; }

        /// <summary>
        /// Points to a <see cref="SoundData"/> unit defining the sound of the blinker relay opening.
        /// </summary>
        [SiiAttribute("blinker_off"), SoundAttribute(SoundAttribute.BlinkerOff)]
        public SoundData BlinkerOff { get; private set; }

        /// <summary>
        /// Points to a <see cref="SoundData"/> unit defining the sound of the wiper(s) traveling 
        /// from the park position to the extended position.
        /// </summary>
        [SiiAttribute("wipers_up"), SoundAttribute(SoundAttribute.WiperUp)]
        public SoundData WipersUp { get; private set; }

        /// <summary>
        /// Points to a <see cref="SoundData"/> unit defining the sound of the wiper(s) traveling 
        /// from the extended position to the park position.
        /// </summary>
        [SiiAttribute("wipers_down"), SoundAttribute(SoundAttribute.WiperDown)]
        public SoundData WipersDown { get; private set; }

        /// <summary>
        /// Defines whether the unit contains exterior sounds (true) or interior sounds (false).
        /// </summary>
        [SiiAttribute("exterior_sound")]
        public bool Exterior { get; private set; }

        [SiiAttribute("suitable_for")]
        public string[] SuitableEngines { get; private set; }
    }
}
