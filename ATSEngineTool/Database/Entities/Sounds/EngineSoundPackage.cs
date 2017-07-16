using System.Collections.Generic;
using System.Linq;
using CrossLite.CodeFirst;

namespace ATSEngineTool.Database
{
    /// <summary>
    /// Represents a Engine Sound Package
    /// </summary>
    /// <remarks>
    /// Database columns are inherited from <see cref="SoundPackage"/>
    /// </remarks>
    [Table]
    public class EngineSoundPackage : SoundPackage
    {
        /// <summary>
        /// Gets a list of <see cref="EngineSeries"/> that reference this 
        /// <see cref="SoundPackage"/>
        /// </summary>
        /// <remarks>
        /// A lazy loaded enumeration that fetches all EngineSeries
        /// that are bound by the foreign key and this SoundPackage.Id.
        /// </remarks>
        public virtual IEnumerable<EngineSeries> Series { get; set; }

        /// <summary>
        /// Gets a list of <see cref="EngineSound"/> entities that reference this 
        /// <see cref="SoundPackage"/>
        /// </summary>
        public virtual IEnumerable<EngineSound> EngineSounds { get; set; }

        /// <summary>
        /// Gets a list of <see cref="Truck"/> entities that reference this 
        /// <see cref="SoundPackage"/>
        /// </summary>
        /// <remarks>
        /// A lazy loaded enumeration that fetches all EngineSeries
        /// that are bound by the foreign key and this SoundPackage.Id.
        /// </remarks>
        public virtual IEnumerable<TruckSoundSetting> TruckSettings { get; set; }

        /// <summary>
        /// Gets the <see cref="Database.SoundType"/> of the sounds that fall under this sound package
        /// </summary>
        public override SoundType SoundType => SoundType.Engine;

        /// <summary>
        /// Gets the folder name where sound packages of this <see cref="SoundType"/>
        /// will be stored.
        /// </summary>
        public override string PackageTypeFolderName => "engine";

        /// <summary>
        /// Gets a list of sounds that fall under this sound package
        /// </summary>
        public override List<Sound> GetSounds()
        {
            return EngineSounds.Select(x => (Sound)x).ToList();
        }
    }
}
