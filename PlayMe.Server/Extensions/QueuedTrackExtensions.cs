using System;
using System.Linq;
using PlayMe.Data.NHibernate.Entities;

namespace PlayMe.Server.Extensions
{
    public static class QueuedTrackExtensions
    {
        public static string ToLoggerFriendlyTrackName(this QueuedTrack track)
        {
            if (track == null|| track.Track ==null) return String.Empty;

            var artistName = String.Empty;

            if (track.Track.Artists != null)
            {
                var artist = track.Track.Artists.FirstOrDefault();
                if (artist != null) artistName = artist.Name + ", ";
            }

            return String.Format("{0}{1}", artistName, track.Track);
        }
    }
}
