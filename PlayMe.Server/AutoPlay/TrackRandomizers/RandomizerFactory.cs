using System.Collections.Generic;
using System.Linq;
using PlayMe.Common.Model;
using PlayMe.Data;
using PlayMe.Plumbing.Diagnostics;
using PlayMe.Server.AutoPlay.TrackRandomizers.Interfaces;
using PlayMe.Server.Providers;

namespace PlayMe.Server.AutoPlay.TrackRandomizers
{
    public interface IRandomizerFactory
    {
        ITrackRandomizer Randomize { get; }
    }

    public class RandomizerFactory : IRandomizerFactory
    {
        private readonly IEnumerable<ITrackRandomizer> randomizers;
        private readonly ILogger logger;
        private readonly Settings settings;

        public RandomizerFactory(IMusicProviderFactory musicProviderFactory, ILogger logger, IDataService<QueuedTrack> queuedTrackDataService)
        {
            settings = new Settings();

            logger.Debug("Initialize Randomizerfactory with randomizer #" + settings.Randomizer);
            
            this.logger = logger;

            //this should really use DI but no IEnumerable<T> implemntation yet, perhaps I should put these in a factory
            randomizers = new List<ITrackRandomizer>()
                {
                    new PassThroughRandomizer(logger),
                    new OffSameAlbumRandomizer(musicProviderFactory, logger),
                    new SimilarArtistRandomizer(musicProviderFactory, logger),
                    new OffArtistsAlbumRandomizer(musicProviderFactory, logger)
                };
        }

        public ITrackRandomizer Randomize
        {
            get { return randomizers.FirstOrDefault(p => p.Version == settings.Randomizer); }
        }
    }
}
