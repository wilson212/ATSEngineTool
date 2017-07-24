using Sii;

namespace ATSEngineTool.SiiEntities
{
    /// <summary>
    /// Defines a container to load manifest.sii files from sound packages
    /// </summary>
    [SiiUnit("sound_package")]
    public class SoundPackManifest
    {
        /// <summary>
        /// Gets the display name of the sound package
        /// </summary>
        [SiiAttribute("display_name")]
        public string Name { get; private set; }

        /// <summary>
        /// Gets the SoundPackage version
        /// </summary>
        [SiiAttribute("package_version")]
        public string Version { get; private set; }

        /// <summary>
        /// Gets the author's name, whom created the sound package
        /// </summary>
        [SiiAttribute("author")]
        public string Author { get; private set; }

        /// <summary>
        /// Depreciated!
        /// </summary>
        [SiiAttribute("icon")]
        public string Icon { get; private set; }

        /// <summary>
        /// Gets the suggested Interior file name for the sound file
        /// </summary>
        [SiiAttribute("default_interior_filename")]
        public string InteriorName { get; private set; }

        /// <summary>
        /// Gets the suggested Exterior file name for the sound file
        /// </summary>
        [SiiAttribute("default_exterior_filename")]
        public string ExteriorName { get; private set; }

        /// <summary>
        /// Defines whether to include the @include common_sound_*_data.sui directive present
        /// in most sound files.
        /// </summary>
        [SiiAttribute("use_common_sound_sui")]
        public bool UseDirectives { get; private set; } = true;

        /// <summary>
        /// Gets the youtube video ID for the client to listen/view the sounds in action
        /// </summary>
        [SiiAttribute("youtube_video_id")]
        public string YoutubeVideoId { get; private set; }
    }
}
