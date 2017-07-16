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
        /// Gets the sound type
        /// </summary>
        public SoundType SoundType { get; protected set; }

        /// <summary>
        /// Indicates whether this sound attribute is an array
        /// </summary>
        public bool IsArray { get; protected set; } = false;

        /// <summary>
        /// Indicates whether the attribute is of type <see cref="SiiEntities.SoundEngineData"/>
        /// or <see cref="SiiEntities.SoundData"/>
        /// </summary>
        public bool IsEngineSoundData { get; protected set; } = false;

        /// <summary>
        /// Indicates wether this sound array is indexed (only works if IsArray is true).
        /// </summary>
        public bool Indexed { get; protected set; } = false;

        /// <summary>
        /// Indicates whether an empty line should follow this sound attribute
        /// when adding this <see cref="AttributeType"/> to a <see cref="SiiFileBuilder"/>
        /// </summary>
        public bool AppendLineAfter { get; protected set; } = false;

        /// <summary>
        /// Gets a list of all supported sound attributes
        /// </summary>
        public static ReadOnlyDictionary<SoundAttribute, SoundInfo> Attributes { get; protected set; }

        /// <summary>
        /// A private constructor
        /// </summary>
        private SoundInfo(SoundAttribute attr, SoundType type, string attrName, string structName, bool array = false, bool space = false)
        {
            this.AttributeType = attr;
            this.AttributeName = attrName;
            this.SoundType = type;
            this.StructName = structName;
            this.IsArray = array;
            this.AppendLineAfter = space;
        }

        /// <summary>
        /// Create the sound attributes array
        /// </summary>
        static SoundInfo()
        {
            var attributes = new List<SoundInfo>();
            attributes.Add(new SoundInfo(SoundAttribute.Start, SoundType.Engine, "start", ".start"));
            attributes.Add(new SoundInfo(SoundAttribute.StartNoFuel, SoundType.Engine, "start_no_fuel", ".startbad"));
            attributes.Add(new SoundInfo(SoundAttribute.Stop, SoundType.Engine, "stop", ".stop", space: true));

            attributes.Add(new SoundInfo(SoundAttribute.Engine, SoundType.Engine, "engine", ".e", true, true)
            {
                IsEngineSoundData = true
            });
            attributes.Add(new SoundInfo(SoundAttribute.EngineLoad, SoundType.Engine, "engine_load", ".el", true, true)
            {
                IsEngineSoundData = true
            });
            attributes.Add(new SoundInfo(SoundAttribute.EngineNoFuel, SoundType.Engine, "engine_nofuel", ".enf", true, true)
            {
                IsEngineSoundData = true
            });
            attributes.Add(new SoundInfo(SoundAttribute.EngineExhaust, SoundType.Engine, "engine_exhaust", ".ee", true, true)
            {
                IsEngineSoundData = true
            });

            attributes.Add(new SoundInfo(SoundAttribute.Turbo, SoundType.Engine, "turbo", ".t", space: true));
            attributes.Add(new SoundInfo(SoundAttribute.AirGears, SoundType.Truck, "air_gear", ".airgear", true, true)
            {
                Indexed = true
            });
            attributes.Add(new SoundInfo(SoundAttribute.AirBrakes, SoundType.Truck, "air_brake", ".airbrake", true, true)
            {
                Indexed = true
            });

            attributes.Add(new SoundInfo(SoundAttribute.EngineBrake, SoundType.Engine, "engine_brake", ".eb", true, true)
            {
                IsEngineSoundData = true
            });
            attributes.Add(new SoundInfo(SoundAttribute.Horn, SoundType.Truck, "horn", ".horn"));
            attributes.Add(new SoundInfo(SoundAttribute.AirHorn, SoundType.Truck, "air_horn", ".ahorn"));
            attributes.Add(new SoundInfo(SoundAttribute.Reverse, SoundType.Truck, "reverse", ".reverse", space: true));

            attributes.Add(new SoundInfo(SoundAttribute.BlinkerOn, SoundType.Truck, "blinker_on", ".blinker_on"));
            attributes.Add(new SoundInfo(SoundAttribute.BlinkerOff, SoundType.Truck, "blinker_off", ".blinker_off"));
            attributes.Add(new SoundInfo(SoundAttribute.WiperUp, SoundType.Truck, "wipers_up", ".wipers_up"));
            attributes.Add(new SoundInfo(SoundAttribute.WiperDown, SoundType.Truck, "wipers_down", ".wipers_down", space: true));

            // A common sound, shouldnt be here but has to be...
            attributes.Add(new SoundInfo(SoundAttribute.ChangeGear, SoundType.Truck, "change_gear", ".changeg", space: true));

            // Set to readonly
            Attributes = new ReadOnlyDictionary<SoundAttribute, SoundInfo>(
                attributes.ToDictionary(x => x.AttributeType, y => y)
            );
        }
    }
}
