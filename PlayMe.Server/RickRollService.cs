using System;
using System.Collections.Generic;
using System.Linq;
using PlayMe.Data;
using PlayMe.Data.NHibernate.Entities;
using PlayMe.Plumbing.Diagnostics;
using PlayMe.Server.Interfaces;
using PlayMe.Server.Providers;

namespace PlayMe.Server
{
    public class RickRollService : IRickRollService
    {        
        private readonly IRepository<RickRoll> _rickRollRepository;
        private readonly IMusicProviderFactory musicProviderFactory;
        private readonly ILogger logger;

        public RickRollService(IRepository<RickRoll> _rickRollRepository,IMusicProviderFactory musicProviderFactory,ILogger logger)
        {
            this.logger = logger;
            this.musicProviderFactory = musicProviderFactory;
            this._rickRollRepository = _rickRollRepository;
        }

        public Track RickRoll(Track track, string user)
        {
            logger.Trace("RickRoll");
            if((track.Artists!=null && track.Artists.Any(a => RickRollExists(a.Link))) || (track.Album !=null && RickRollExists(track.Album.Link)) || RickRollExists(track.Link))            
            {
                const string rickRollTrackId = "6JEK0CvvjDjjMUBFoXShNZ";
                var rickRollTrack = musicProviderFactory.GetMusicProviderByIdentifier(track.MusicProvider.Identifier).GetTrack(rickRollTrackId, user);
                //Create a new track that looks like the the original but is in fact 'Never give you up'
                var rickRolledTrack = new Track
                                      {
                                          Link = rickRollTrack.Link,
                                          Name = track.Name,
                                          Album = track.Album,
                                          Artists = track.Artists,
                                          Duration = rickRollTrack.Duration,
                                          DurationMilliseconds = (long)rickRollTrack.Duration.TotalMilliseconds,
                                          IsAvailable = track.IsAvailable,                                          
                                      };
                logger.Debug("Rick rolling track {0}", track);
                return rickRolledTrack;
            }
            return track;
        }

        public RickRoll AddRickRoll(PlayMeObject playMeObject)
        {
            var rickRoll = new RickRoll
                               {
                                   PlayMeObject = playMeObject
                               };
            _rickRollRepository.Insert(rickRoll);
            return rickRoll;
        }

        public void RemoveRickRoll(Guid id)
        {
            _rickRollRepository.Delete(_rickRollRepository.Get(id));
        }
        
        public IEnumerable<RickRoll> GetAllRickRolls()
        {
            //Order by name
            return _rickRollRepository.GetAll().OrderBy(r => r.PlayMeObject.Name);
        }

        private bool RickRollExists(string id)
        {
            return _rickRollRepository.GetAll().Any(r => r.PlayMeObject.Link == id);
        }

    }
}
