using System.Collections.Generic;
using System.IO;
using ATSEngineTool.Database;

namespace ATSEngineTool
{
    public class SoundWrapper
    {
        public int ChildCount => Children.Count;

        public List<SoundWrapper> Children { get; protected set; } = new List<SoundWrapper>();

        public string SoundName { get; set; } = string.Empty;

        public SoundType Catagory => Sound.Type;

        public string Filename
        {
            get
            {
                return (Children.Count > 0) ? string.Empty : Path.GetFileName(Sound.FileName);
            }
        }

        public string Volume
        {
            get
            {
                return (Children.Count > 0) ? string.Empty : (Sound.Volume * 100) + "%";
            }
        }

        public SoundWrapper Parent { get; set; }

        public EngineSound Sound { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is SoundWrapper)
            {
                return ((SoundWrapper)obj).SoundName == this.SoundName;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}