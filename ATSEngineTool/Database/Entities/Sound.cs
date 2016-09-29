using CrossLite;
using CrossLite.CodeFirst;

namespace ATSEngineTool.Database
{
    [Table]
    [CompositeUnique("PackageId", "FileName")]
    public class Sound
    {
        [Column, PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Column, Required]
        public int PackageId { get; set; }

        [Column, Required]
        public SoundAttribute Attribute { get; set; }

        [Column, Required, Default(0)]
        public SoundType Type { get; set; }

        [Column, Required]
        public string FileName { get; private set; }

        [Column, Required, Default(false)]
        public bool Looped { get; private set; }

        [Column, Required, Default(false)]
        public bool Is2D { get; private set; }

        [Column, Required]
        public double Volume { get; private set; }
    }
}
