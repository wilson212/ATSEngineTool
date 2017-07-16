using System.Collections.Generic;
using System.Linq;
using CrossLite.CodeFirst;

namespace ATSEngineTool.Database
{
    /// <summary>
    /// Represents a Truck Sound Package
    /// </summary>
    /// <remarks>
    /// Database columns are inherited from <see cref="SoundPackage"/>
    /// </remarks>
    [Table]
    public class TruckSoundPackage : SoundPackage
    {
        /// <summary>
        /// Gets a list of <see cref="Truck"/> entities that reference this 
        /// <see cref="SoundPackage"/>
        /// </summary>
        public virtual IEnumerable<Truck> Trucks { get; set; }

        /// <summary>
        /// Gets a list of <see cref="TruckSound"/> entities that reference this 
        /// <see cref="SoundPackage"/>
        /// </summary>
        public virtual IEnumerable<TruckSound> TruckSounds { get; set; }

        /// <summary>
        /// Gets the <see cref="Database.SoundType"/> of the sounds that fall under this sound package
        /// </summary>
        public override SoundType SoundType => SoundType.Truck;

        /// <summary>
        /// Gets the folder name where sound packages of this <see cref="SoundType"/>
        /// will be stored.
        /// </summary>
        public override string PackageTypeFolderName => "common";

        /// <summary>
        /// Gets a list of sounds that fall under this sound package
        /// </summary>
        /// <returns></returns>
        public override List<Sound> GetSounds()
        {
            return TruckSounds.Select(x => (Sound)x).ToList();
        }
    }
}
