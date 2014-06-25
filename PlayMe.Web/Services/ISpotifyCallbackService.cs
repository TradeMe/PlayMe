using System.Collections.Generic;
using System.ServiceModel;

namespace PlayMe.Web.Services
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "ISpotifyCallbackService" in both code and config file together.
    [ServiceContract]
    public interface ISpotifyCallbackService
    {
        [OperationContract(IsOneWay = true)]
        void PlayingTrackChanged(QueuedTrack track);

        [OperationContract(IsOneWay = true)]
        void QueueChanged(IEnumerable<QueuedTrack> queue);

        [OperationContract(IsOneWay = true)]
        void TrackHistoryChanged(PagedResult<QueuedTrack> history);
    }
}
