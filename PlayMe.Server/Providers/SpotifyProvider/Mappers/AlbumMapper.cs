using System;
using PlayMe.Common.Model;
using S = SpotiFire.SpotifyLib;

namespace PlayMe.Server.Providers.SpotifyProvider.Mappers
{
    public class AlbumMapper : IAlbumMapper
    {
        private readonly IArtistMapper artistMapper;
        
        public AlbumMapper(IArtistMapper artistMapper)
        {
            this.artistMapper = artistMapper;
        }

        public Album Map(S.IAlbum album, SpotifyMusicProvider musicProvider, bool mapArtist = false)
        {
            var albumLink = album.GetLinkString();
            var albumResult = new Album
            {
                Link = albumLink,
                Name = album.Name,
                Year = album.Year,
                ArtworkId = album.CoverId,
                ArtworkUrlLarge = ImageHelper.GetImageUrl(album.CoverId, 300),
                ArtworkUrlMedium = ImageHelper.GetImageUrl(album.CoverId, 120),
                ArtworkUrlSmall = ImageHelper.GetImageUrl(album.CoverId, 60),
                IsAvailable = album.IsAvailable,
                MusicProvider = musicProvider.Descriptor,
                ExternalLink = new Uri(musicProvider.ExternalLinkBaseUrl, "/album/" + albumLink)
            };
            if (mapArtist && album.Artist != null)
            {
                albumResult.Artist = artistMapper.Map(album.Artist, musicProvider);
            }
            return albumResult;

        }
    }
}