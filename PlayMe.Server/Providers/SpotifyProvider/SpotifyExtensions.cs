using SpotiFire.SpotifyLib;

namespace PlayMe.Server.Providers.SpotifyProvider
{
    public static class SpotifyExtensions
    {
        
        #region Link extensions
        
        public static string ToLinkString(this IArtist artist)
        {
            ILink l = artist.CreateLink();
            return l.ToTrimmedString();
        }

        public static string ToLinkString(this IAlbum album)
        {
            ILink l = album.CreateLink();
            return l.ToTrimmedString();
        }

        public static string ToLinkString(this ITrack track)
        {
            ILink l = track.CreateLink();
            return l.ToTrimmedString();            
        }

        private static string ToTrimmedString(this ILink link)
        {
            string linkAsString = link.ToString();
            return linkAsString.Substring(linkAsString.LastIndexOf(':') + 1);
        }

        #endregion

        //public static async Task<Track> AsTask(this Track track)
        //{
        //    return await track;
        //}

        //public static async Task<Artist> AsTask(this Artist artist)
        //{
        //    return await artist;
        //}

        //public static async Task<Album> AsTask(this Album album)
        //{
        //    return await album;
        //}

        //public static async Task<Search> AsTask(this Search search)
        //{
        //    return await search;
        //}

        //public static async Task<AlbumBrowse> AsTask(this AlbumBrowse albumBrowse)
        //{
        //    return await albumBrowse;
        //}

        //public static async Task<ArtistBrowse> AsTask(this ArtistBrowse artistBrowse)
        //{
        //    return await artistBrowse;
        //}

        public static string GetLinkString(this IArtist artist)
        {
            return RemovePrefix(artist.CreateLink().ToString());
        }

        public static string GetLinkString(this IAlbum album)
        {
            return RemovePrefix(album.CreateLink().ToString());
        }

        public static string GetLinkString(this ITrack track)
        {
            return RemovePrefix(track.CreateLink().ToString());
        }

        private static string RemovePrefix(string link)
        {
            return link.Substring(link.LastIndexOf(':') + 1);
        }
    }
}