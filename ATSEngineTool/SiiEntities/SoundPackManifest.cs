using Sii;

namespace ATSEngineTool.SiiEntities
{
    [SiiUnit("sound_package")]
    public class SoundPackManifest
    {
        [SiiAttribute("display_name")]
        public string Name { get; private set; }

        [SiiAttribute("package_version")]
        public string Version { get; private set; }

        [SiiAttribute("author")]
        public string Author { get; private set; }

        [SiiAttribute("icon")]
        public string Icon { get; private set; }

        [SiiAttribute("default_interior_filename")]
        public string InteriorName { get; private set; }

        [SiiAttribute("default_exterior_filename")]
        public string ExteriorName { get; private set; }

        [SiiAttribute("use_common_sound_sui")]
        public bool UseDirectives { get; private set; } = true;
    }
}
