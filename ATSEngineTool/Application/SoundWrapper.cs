using System.Collections.Generic;
using ATSEngineTool.Database;

namespace ATSEngineTool
{
    public class SoundWrapper
    {
        public int ChildCount => Children.Count;

        public List<SoundWrapper> Children { get; protected set; } = new List<SoundWrapper>();

        public string SoundName { get; set; }

        public string Filename { get; set; } = string.Empty;

        public string Volume { get; set; } = string.Empty;

        public SoundWrapper Parent { get; set; }

        public EngineSound Sound { get; set; }
    }
}
