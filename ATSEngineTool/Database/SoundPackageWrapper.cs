using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ATSEngineTool.Database
{
    public class SoundPackageWrapper<TPack, TSound> 
        where TPack : SoundPackage 
        where TSound : Sound
    {
        public TPack Package { get; set; }

        protected Dictionary<SoundLocation, Dictionary<SoundAttribute, List<TSound>>> Sounds { get; set; }

        private static SoundLocation[] Locations { get; set; }

        static SoundPackageWrapper()
        {
            Locations = Enum.GetValues(typeof(SoundLocation)).Cast<SoundLocation>().ToArray();
        }

        public SoundPackageWrapper(TPack package)
        {
            Package = package;
            Sounds = new Dictionary<SoundLocation, Dictionary<SoundAttribute, List<TSound>>>();

            // Add sound locations
            foreach (var location in Locations)
            {
                Sounds.Add(location, new Dictionary<SoundAttribute, List<TSound>>());
            }

            // Cache sounds
            var sounds = package.GetSounds().Select(x => (TSound)x).OrderBy(x => x.Attribute).ThenBy(x => x.Id);
            foreach (TSound sound in sounds)
            {
                var sublet = Sounds[sound.Location];
                if (!sublet.ContainsKey(sound.Attribute))
                    sublet.Add(sound.Attribute, new List<TSound>());

                sublet[sound.Attribute].Add(sound);
            }
        }

        public Dictionary<SoundAttribute, List<TSound>> GetSoundsByLocation(SoundLocation location)
        {
            return Sounds[location];
        }
    }
}
