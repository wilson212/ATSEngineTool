using System.Collections.Generic;
using ATSEngineTool.Database;

namespace ATSEngineTool
{
    public class SoundWrapper
    {
        public int ChildCount => Children.Count;

        public List<SoundWrapper> Children { get; protected set; } = new List<SoundWrapper>();

        public string SoundName { get; set; }

        public string Filename
        {
            get
            {
                return (Children.Count > 0) ? string.Empty : Sound.FileName;
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
    }
}