using System;
using System.Collections.Generic;
using System.Linq;
using PlayMe.Common.Model;
using PlayMe.Data;
using PlayMe.Plumbing.Diagnostics;
using PlayMe.Server.Interfaces;
using PlayMe.Server.Providers;

namespace PlayMe.Server
{
    public class RickRollService : IRickRollService
    {        
        private readonly IDataService<RickRoll> rickRollDataService;
        private readonly IMusicProviderFactory musicProviderFactory;
        private readonly ILogger logger;

        public RickRollService(IDataService<RickRoll> rickRollDataService,IMusicProviderFactory musicProviderFactory,ILogger logger)
        {
            this.logger = logger;
            this.musicProviderFactory = musicProviderFactory;
            this.rickRollDataService = rickRollDataService;
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
            rickRollDataService.Insert(rickRoll);
            return rickRoll;
        }

        public void RemoveRickRoll(Guid id)
        {
            rickRollDataService.Delete(rickRollDataService.Get(id));
        }
        
        public IEnumerable<RickRoll> GetAllRickRolls()
        {
            //Order by name
            return rickRollDataService.GetAll().OrderBy(r => r.PlayMeObject.Name);
        }

        private bool RickRollExists(string id)
        {
            return rickRollDataService.GetAll().Any(r => r.PlayMeObject.Link == id);
        }

    }
}
