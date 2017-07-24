using Sii;

namespace ATSEngineTool.SiiEntities
{
    /// <summary>
    /// Defines engine sounds
    /// </summary>
    /// <seealso cref="http://modding.scssoft.com/wiki/Documentation/Engine/Units/sound_engine_data"/>
    [SiiUnit("sound_engine_data")]
    public sealed class SoundEngineData
    {
        /// <summary>
        /// Gets the path to the sound clip
        /// </summary>
        [SiiAttribute("name")]
        public string Name { get; private set; }

        /// <summary>
        /// When true, the sound clip will be looped while active. When false, it will play once when triggered. 
        /// Should always true for engine sounds.
        /// </summary>
        [SiiAttribute("looped")]
        public bool Looped { get; private set; }

        /// <summary>
        /// Gets the reference engine rpm of the recording.
        /// </summary>
        [SiiAttribute("pitch_reference")]
        public double Pitch { get; private set; }

        /// <summary>
        /// Gets the highest rpm the clip will be played at.
        /// </summary>
        [SiiAttribute("max_rpm")]
        public double MaxRPM { get; private set; }

        /// <summary>
        /// Gets the lowest rpm the clip will be played at.
        /// </summary>
        [SiiAttribute("min_rpm")]
        public double MinRPM { get; private set; }

        /// <summary>
        /// Gets the playback volume relative to recorded level
        /// </summary>
        [SiiAttribute("volume")]
        public double Volume { get; private set; } = 1.0;

        [SiiAttribute("is_2d")]
        public bool Is2D { get; private set; }
    }
}
