using Sii;

namespace ATSEngineTool
{
    [SiiUnit("sound_data")]
    internal sealed class SoundData
    {
        [SiiAttribute("name")]
        public string Name { get; private set; }

        [SiiAttribute("looped")]
        public bool Looped { get; private set; }

        [SiiAttribute("volume")]
        public double Volume { get; private set; }
    }
}
