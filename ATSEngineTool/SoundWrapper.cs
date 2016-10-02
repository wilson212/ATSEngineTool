using System.Collections.Generic;

namespace ATSEngineTool
{
    public class SoundWrapper
    {
        public int ChildCount => Children.Count;

        public List<SoundWrapper> Children { get; protected set; } = new List<SoundWrapper>();

        public string Label { get; set; }

        public SoundWrapper Parent { get; set; }
    }
}
