using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ATSEngineTool.Database;

namespace ATSEngineTool
{
    public class SoundInfo
    {
        /// <summary>
        /// Gets the attribute name
        /// </summary>
        public string AttributeName { get; protected set; }

        /// <summary>
        /// Gets the name of the nameless sound object
        /// </summary>
        public string StructName { get; protected set; }

        /// <summary>
        /// Gets the sound attribute type
        /// </summary>
        public SoundAttribute AttributeType { get; protected set; }

        /// <summary>
        /// Indicates whether this sound attribute is an array
        /// </summary>
        public bool IsArray { get; protected set; } = false;

        /// <summary>
        /// Indicates wether this sound array is indexed (only works if IsArray is true).
        /// </summary>
        public bool Indexed { get; protected set; } = false;

        /// <summary>
        /// Indicates whether this sound attribute is an engine sound or truck sound
        /// </summary>
        public bool IsEngineSound { get; protected set; } = false;

        /// <summary>
        /// Indicates whether an empty line should follow this sound attribute
        /// </summary>
        public bool AppendLineAfter { get; protected set; } = false;

        /// <summary>
        /// Gets a list of all supported sound attributes
        /// </summary>
        public static ReadOnlyDictionary<SoundAttribute, SoundInfo> Attributes { get; protected set; }

        /// <summary>
        /// A private constructor
        /// </summary>
        private SoundInfo(SoundAttribute type, string name, string sname, bool array = false, bool engine = false, bool space = false)
        {
            this.AttributeType = type;
            this.AttributeName = name;
            this.StructName = sname;
            this.IsArray = array;
            this.IsEngineSound = engine;
            this.AppendLineAfter = space;
        }

        /// <summary>
        /// Create the sound attributes array
        /// </summary>
        static SoundInfo()
        {
            var attributes = new List<SoundInfo>();
            attributes.Add(new SoundInfo(SoundAttribute.Start, "start", ".start"));
            attributes.Add(new SoundInfo(SoundAttribute.StartNoFuel, "start_no_fuel", ".startbad"));
            attributes.Add(new SoundInfo(SoundAttribute.Stop, "stop", ".stop", space: true));

            attributes.Add(new SoundInfo(SoundAttribute.Engine, "engine", ".e", true, true, true));
            attributes.Add(new SoundInfo(SoundAttribute.EngineLoad, "engine_load", ".el", true, true, true));
            attributes.Add(new SoundInfo(SoundAttribute.EngineNoFuel, "engine_nofuel", ".enf", true, true, true));
            attributes.Add(new SoundInfo(SoundAttribute.EngineExhaust, "engine_exhaust", ".ee", true, true, true));

            attributes.Add(new SoundInfo(SoundAttribute.Turbo, "turbo", ".t", space: true));
            attributes.Add(new SoundInfo(SoundAttribute.AirGears, "air_gear", ".airgear", true, false, true)
            {
                Indexed = true
            });
            attributes.Add(new SoundInfo(SoundAttribute.AirBrakes, "air_brake", ".airbrake", true, false, true)
            {
                Indexed = true
            });

            attributes.Add(new SoundInfo(SoundAttribute.EngineBrake, "engine_brake", ".eb", true, true, true));
            attributes.Add(new SoundInfo(SoundAttribute.Horn, "horn", ".horn"));
            attributes.Add(new SoundInfo(SoundAttribute.AirHorn, "air_horn", ".ahorn"));
            attributes.Add(new SoundInfo(SoundAttribute.Reverse, "reverse", ".reverse", space: true));

            attributes.Add(new SoundInfo(SoundAttribute.BlinkerOn, "blinker_on", ".blinker_on"));
            attributes.Add(new SoundInfo(SoundAttribute.BlinkerOff, "blinker_off", ".blinker_off"));
            attributes.Add(new SoundInfo(SoundAttribute.WiperUp, "wipers_up", ".wipers_up"));
            attributes.Add(new SoundInfo(SoundAttribute.WiperDown, "wipers_down", ".wipers_down", space: true));

            // A common sound, shouldnt be here but has to be...
            attributes.Add(new SoundInfo(SoundAttribute.ChangeGear, "change_gear", ".changeg", space: true));

            // Set to readonly
            Attributes = new ReadOnlyDictionary<SoundAttribute, SoundInfo>(
                attributes.ToDictionary(x => x.AttributeType, y => y)
            );
        }
    }
}
