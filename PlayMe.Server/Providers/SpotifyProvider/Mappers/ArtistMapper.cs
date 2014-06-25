using System;
using PlayMe.Data.NHibernate.Entities;
using S = SpotiFire.SpotifyLib;

namespace PlayMe.Server.Providers.SpotifyProvider.Mappers
{
    public class ArtistMapper : IArtistMapper
    {
        public Artist Map(S.IArtist artist, SpotifyMusicProvider musicProvider)
        {
            string artistLink = artist.GetLinkString();
            var artistResult = new Artist
            {
                Link = artistLink,
                Name = artist.Name,
                PortraitId = artist.PortraitId,
                PortraitUrlLarge = ImageHelper.GetImageUrl(artist.PortraitId, 300),
                PortraitUrlMedium = ImageHelper.GetImageUrl(artist.PortraitId, 120),
                PortraitUrlSmall = ImageHelper.GetImageUrl(artist.PortraitId, 60),
                MusicProvider = musicProvider.Descriptor,
                ExternalLink = new Uri(musicProvider.ExternalLinkBaseUrl, "/artist/ " + artistLink)
            };

            return artistResult;

        }
    }
}