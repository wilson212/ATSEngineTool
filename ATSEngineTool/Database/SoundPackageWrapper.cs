using System;
using System.Collections.Generic;
using System.Linq;

namespace ATSEngineTool.Database
{
    /// <summary>
    /// This class is used to cache the <see cref="Sound"/>s contained in a sound package
    /// </summary>
    /// <typeparam name="TPack"></typeparam>
    /// <typeparam name="TSound"></typeparam>
    public class SoundPackageWrapper<TPack, TSound> 
        where TPack : SoundPackage 
        where TSound : Sound
    {
        /// <summary>
        /// Gets or sets the Sound Package contained in this wrapper
        /// </summary>
        public TPack Package { get; set; }

        /// <summary>
        /// locaiton => [Attribute => List{TSound}]
        /// </summary>
        protected Dictionary<SoundLocation, Dictionary<SoundAttribute, List<TSound>>> Sounds { get; set; }

        /// <summary>
        /// An array of all sound locations
        /// </summary>
        private static SoundLocation[] Locations { get; set; }

        static SoundPackageWrapper()
        {
            Locations = Enum.GetValues(typeof(SoundLocation)).Cast<SoundLocation>().ToArray();
        }

        /// <summary>
        /// Creates a new instance of <see cref="SoundPackageWrapper{TPack, TSound}"/>
        /// </summary>
        /// <param name="package"></param>
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

        /// <summary>
        /// Gets a dictionary of sounds contained in this package based on location
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public Dictionary<SoundAttribute, List<TSound>> GetSoundsByLocation(SoundLocation location)
        {
            return Sounds[location];
        }
    }
}
