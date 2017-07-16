using System;
using System.Collections.Generic;
using System.IO;
using CrossLite;
using CrossLite.CodeFirst;

namespace ATSEngineTool.Database
{
    public abstract class SoundPackage : IEquatable<SoundPackage>
    {
        /// <summary>
        /// Gets or Sets the Unique id for this entity
        /// </summary>
        [Column, PrimaryKey]
        public int Id { get; protected set; }

        /// <summary>
        /// Gets or Sets the unique name for this Sound
        /// </summary>
        [Column, Required, Unique]
        public string Name { get; set; }

        /// <summary>
        /// Gets or Sets the folder name that this sound package is located in.
        /// </summary>
        [Column, Required, Unique, Collation(Collation.NoCase)]
        public string FolderName { get; set; }

        /// <summary>
        /// Gets or Sets the filename that this <see cref="SoundPackage"/> will use
        /// when generating the interior sound file.
        /// </summary>
        [Column, Required, Default("")]
        public string InteriorFileName { get; set; } = String.Empty;

        /// <summary>
        /// Gets or Sets the filename that this <see cref="SoundPackage"/> will use
        /// when generating the exterior sound file.
        /// </summary>
        [Column, Required, Default("")]
        public string ExteriorFileName { get; set; } = String.Empty;

        /// <summary>
        /// Gets or Sets the unique name for this Sound
        /// </summary>
        [Column, Required]
        public string UnitName { get; set; }

        /// <summary>
        /// Gets or Sets the author of this sound package
        /// </summary>
        [Column, Required, Default("")]
        public string Author { get; set; }

        /// <summary>
        /// Gets or Sets the package version number
        /// </summary>
        [Column, Required, Default("1.0")]
        public string Version { get; set; }

        /// <summary>
        /// Gets or Sets the YouTube video id for this sound package
        /// </summary>
        [Column, Required, Default("")]
        public string YoutubeVideoId { get; set; } = String.Empty;

        /// <summary>
        /// Gets the <see cref="Database.SoundType"/> of the sounds that fall under this sound package
        /// </summary>
        public abstract SoundType SoundType { get; }

        /// <summary>
        /// Gets the folder name where sound packages of this <see cref="SoundType"/>
        /// will be stored.
        /// </summary>
        public abstract string PackageTypeFolderName { get; }

        /// <summary>
        /// Gets the full folder path from the sound package's root folder,
        /// using forward slashes as directory the seperator
        /// </summary>
        public string PackageGamePath => $"/sound/truck/{PackageTypeFolderName}/{FolderName}";

        /// <summary>
        /// Gets the relative folder path from the sound package's root folder,
        /// using forward the system directory seperator.
        /// </summary>
        public string RelativeSystemPath => $"{PackageTypeFolderName}{Path.DirectorySeparatorChar}{FolderName}";

        /// <summary>
        /// Gets a list of sounds that fall under this sound package
        /// </summary>
        public abstract List<Sound> GetSounds();  

        #region overrides

        public override string ToString() => Name;

        public bool Equals(SoundPackage other)
        {
            if (other == null) return false;
            return other.Id == Id;
        }

        public override bool Equals(object obj) => Equals(obj as SoundPackage);

        public override int GetHashCode() => this.Id.GetHashCode();

        #endregion overrides
    }
}
