using Sii;

namespace ATSEngineTool.SiiEntities
{
    /// <summary>
    /// Defines a base class for all accessory_*_data classes and defines 
    /// basic UI attributes and compatibility/interactions between accessories. 
    /// </summary>
    /// <seealso cref="http://modding.scssoft.com/wiki/Documentation/Engine/Units/accessory_data"/>
    public abstract class AccessoryData
    {
        /// <summary>
        /// Gets the full name of the accessory for UI display.
        /// </summary>
        [SiiAttribute("name")]
        public string Name { get; protected set; }

        /// <summary>
        /// Gets a condensed name of the accessory for UI display in contexts with limited space 
        /// or where full names are otherwise ill-suited.
        /// </summary>
        [SiiAttribute("short_name")]
        public string ShortName { get; protected set; }

        /// <summary>
        /// Gets the price of the accessory in the store in the base currency. 
        /// If zero, the accessory will not be visible to the player.
        /// </summary>
        [SiiAttribute("price")]
        public ushort Price { get; protected set; }

        /// <summary>
        /// Gets the minimum level the player must achieve before the accessory becomes 
        /// available in the store.
        /// </summary>
        [SiiAttribute("unlock")]
        public byte UnlockLevel { get; protected set; }

        /// <summary>
        /// Gets te path relative to "/material/ui/accessory/" to the icon for the accessory, 
        /// omitting the file extension.
        /// </summary>
        [SiiAttribute("icon")]
        public string Icon { get; protected set; }

        /// <summary>
        /// Each array member specifies a path to an accessory definition file which is applied 
        /// to the vehicle if no other suitable accessory of the same type exists on the vehicle.
        /// </summary>
        [SiiAttribute("defaults")]
        public string[] Defaults { get; protected set; }

        /// <summary>
        /// Each array member specifies a unit name — or a pattern using wildcards (*) — 
        /// with which this accessory is incompatible. The conflicting accessories will be 
        /// removed from the vehicle if this accessory is applied.
        /// </summary>
        [SiiAttribute("conflict_with")]
        public string[] Conflicts { get; protected set; }

        /// <summary>
        /// Each array member specifies a unit name — or a pattern using wildcards (*) — 
        /// which must be present on the vehicle for this accessory to become visible in 
        /// the store and applicable to the vehicle.
        /// </summary>
        [SiiAttribute("suitable_for")]
        public string[] Suitables { get; protected set; }

        /// <summary>
        /// Each array member specifies a path to an accessory definition file which is applied to the vehicle, 
        /// overriding other accessories of the same type if one already exists on the vehicle. 
        /// This eliminates the need for suitable_for/defaults relationships in most cases 
        /// (e.g. engine badges and sounds).
        /// </summary>
        [SiiAttribute("overrides")]
        public string[] Overrides { get; protected set; }

        /// <summary>
        /// Each array member specifies an accessory type that must exist on the vehicle while this accessory is installed.
        /// </summary>
        /// <remarks>
        /// Every required accessory type should always have a corresponding accessory of that type in defaults!
        /// </remarks>
        [SiiAttribute("require")]
        public string[] Requires { get; protected set; }
    }
}
