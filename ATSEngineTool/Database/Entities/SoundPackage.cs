using System.Collections.Generic;
using CrossLite;
using CrossLite.CodeFirst;

namespace ATSEngineTool.Database
{
    [Table]
    public class SoundPackage
    {
        /// <summary>
        /// Gets or Sets the Unique id for this entity
        /// </summary>
        [Column, PrimaryKey, AutoIncrement]
        public int Id { get; protected set; }

        /// <summary>
        /// Gets or Sets the unique name for this Sound
        /// </summary>
        [Column, Required, Unique]
        public string Name { get; set; }

        /// <summary>
        /// Gets or Sets the folder name that this sound package is located in.
        /// </summary>
        [Column, Required]
        public string FolderName { get; set; }

        /// <summary>
        /// Gets or Sets the filename that this <see cref="SoundPackage"/> will use
        /// when generating the interior sound file.
        /// </summary>
        [Column, Required]
        public string InteriorFileName { get; set; }

        /// <summary>
        /// Gets or Sets the filename that this <see cref="SoundPackage"/> will use
        /// when generating the exterior sound file.
        /// </summary>
        [Column, Required]
        public string ExteriorFileName { get; set; }

        /// <summary>
        /// Gets a list of <see cref="EngineSeries"/> that reference this 
        /// <see cref="SoundPackage"/>
        /// </summary>
        /// <remarks>
        /// A lazy loaded enumeration that fetches all EngineSeries
        /// that are bound by the foreign key and this SoundPackage.Id.
        /// </remarks>
        public virtual IEnumerable<EngineSeries> Series { get; set; }

        public override string ToString() => Name;

        public override bool Equals(object obj)
        {
            if (obj is SoundPackage)
            {
                SoundPackage compare = (SoundPackage)obj;
                return compare.Id == this.Id;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return this.Id.GetHashCode();
        }
    }
}
