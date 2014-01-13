using System.Collections.Generic;
using System.Linq;
using PlayMe.Common.Model;
using PlayMe.Server.Helpers;

namespace PlayMe.Server.Queries
{
    public static class QueueTrackQueries
    {
        public static IQueryable<QueuedTrack> GetQueuedTracksByUser(this IQueryable<QueuedTrack> source, string user,int start, int limit, out int total)
        {
            var results = source.Where(t => t.StartedPlayingDateTime != null);

            if (!string.IsNullOrEmpty(user)) results = results.Where(t => t.User.StartsWith(user));
            total = results.Count();
            return results.OrderByDescending(t => t.StartedPlayingDateTime)
                   .Skip(start).Take(limit);                        
        }

        public static IEnumerable<Track> GetLikedTracks(this IQueryable<QueuedTrack> source, string user, int start, int limit, out int total)
        {
            var results = source
                .Where(t => t.Likes.Any(v => v.ByUser == user))
                .Select(t => t.Track) // Track instead of QueuedTrack                                
                .Distinct()
                .ToList(); 
            total = results.Count();
            return results.Skip(start).Take(limit);
        }
    }
}
