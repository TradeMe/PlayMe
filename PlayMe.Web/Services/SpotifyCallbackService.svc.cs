using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using Microsoft.AspNet.SignalR;
using PlayMe.Common.Model;
using PlayMe.Web.Hubs;

namespace PlayMe.Web.Services
{
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    public class SpotifyCallbackService : ISpotifyCallbackService
    {

        public void PlayingTrackChanged(QueuedTrack track)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<QueueHub>();
            context.Clients.All.updateCurrentTrack(track);
        }

        public void QueueChanged(IEnumerable<QueuedTrack> queue)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<QueueHub>();
            context.Clients.All.updatePlayingSoon(queue.Take(5));
        }

        public void TrackHistoryChanged(PagedResult<QueuedTrack> history)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<QueueHub>();
            context.Clients.All.updateRecentlyPlayed(history); 
        }
    }
}
