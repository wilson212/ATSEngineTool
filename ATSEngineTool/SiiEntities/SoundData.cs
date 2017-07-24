using Sii;

namespace ATSEngineTool.SiiEntities
{
    /// <summary>
    /// Defines a sound
    /// </summary>
    /// <seealso cref="http://modding.scssoft.com/wiki/Documentation/Engine/Units/sound_data"/>
    [SiiUnit("sound_data")]
    public sealed class SoundData
    {
        /// <summary>
        /// Gets the path to the sound clip
        /// </summary>
        [SiiAttribute("name")]
        public string Name { get; private set; }

        /// <summary>
        /// When true, the sound clip will be looped while active. When false, it will play once when triggered.
        /// </summary>
        [SiiAttribute("looped")]
        public bool Looped { get; private set; }

        /// <summary>
        /// Gets the playback volume relative to recorded level
        /// </summary>
        [SiiAttribute("volume")]
        public double Volume { get; private set; } = 1.0;

        [SiiAttribute("is_2d")]
        public bool Is2D { get; private set; }
    }
}
