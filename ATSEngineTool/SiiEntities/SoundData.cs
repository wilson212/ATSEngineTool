using System;
using Sii;

namespace ATSEngineTool.SiiEntities
{
    [SiiUnit("sound_data")]
    public sealed class SoundData
    {
        [SiiAttribute("name")]
        public string Name { get; private set; }

        [SiiAttribute("looped")]
        public bool Looped { get; private set; }

        [SiiAttribute("volume")]
        public double Volume { get; private set; } = 1.0;

        [SiiAttribute("is_2d")]
        public bool Is2D { get; private set; }
    }
}
