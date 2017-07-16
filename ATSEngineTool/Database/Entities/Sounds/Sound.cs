using CrossLite;
using CrossLite.CodeFirst;

namespace ATSEngineTool.Database
{
    public abstract class Sound
    {
        #region Columns

        /// <summary>
        /// Gets the Row ID for this <see cref="EngineSound"/>
        /// </summary>
        [Column, PrimaryKey]
        public int Id { get; set; }

        /// <summary>
        /// Gets or Sets the <see cref="SoundPackage.Id"/> that this 
        /// <see cref="EngineSound"/> is apart of
        /// </summary>
        [Column, Required]
        public int PackageId { get; set; }

        /// <summary>
        /// Gets or Sets the <see cref="SoundAttribute"/> for this sound
        /// </summary>
        [Column, Required]
        public SoundAttribute Attribute { get; set; }

        /// <summary>
        /// Gets or Sets whether this is an interior sound, exterior sound, or both
        /// </summary>
        [Column, Required]

        public SoundLocation Location { get; set; }

        /// <summary>
        /// Gets or Sets the sound filename this <see cref="EngineSound"/> uses
        /// </summary>
        [Column, Required]
        public string FileName { get; set; }

        /// <summary>
        /// Gets or Sets whether this sound is looped in game, or played just once
        /// </summary>
        [Column, Required, Default(false)]
        public bool Looped { get; set; }

        /// <summary>
        /// Gets or Sets whether this <see cref="EngineSound"/>'s volume changes
        /// based on the players position relevent to the sounds source.
        /// </summary>
        [Column, Required, Default(false)]
        public bool Is2D { get; set; }

        /// <summary>
        /// Gets or Sets the sounds volume in game
        /// </summary>
        [Column, Required, Default("1.0", Quote = false)]
        public double Volume { get; set; } = 1.0;

        /// <summary>
        /// Gets whether this sound is an array Attribute
        /// </summary>
        public bool IsSoundArray => SoundInfo.Attributes[this.Attribute].IsArray;

        public abstract SoundType SoundType { get; }

        #endregion Columns

        /// <summary>
        /// Appends this <see cref="Sound"/> to an open <see cref="SiiFileBuilder"/> object
        /// </summary>
        public abstract void AppendTo(SiiFileBuilder builder, string objectName, SoundPackage package);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="package"></param>
        /// <returns></returns>
        protected string GetParsedFileNamePath(SoundPackage package)
        {
            // Figure out file path
            string file = this.FileName;
            if (this.FileName.StartsWith("@"))
            {
                string directive = this.FileName.Substring(1, 2);
                switch (directive.ToUpperInvariant())
                {
                    case "SP":
                    case "CP":
                    case "EP":
                        file = this.FileName.Replace($"@{directive}", package.PackageGamePath);
                        break;
                    default:
                        file = this.FileName.Replace($"@{directive}", "/sound/truck/default");
                        break;
                }
            }

            return file;
        }
    }
}
