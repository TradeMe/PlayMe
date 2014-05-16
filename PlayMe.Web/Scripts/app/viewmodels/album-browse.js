define(['repositories/albumRepository', 'repositories/queueRepository'], function (albumRepository, queueRepository) {
    var album = ko.observable();
    
    return {
        album: album,

        queueTrack: function (track) {
            track.ReasonExpanded(false);
            queueRepository.queueTrack(track);
        },
        
        expandReason: function (track) {
            if (!track.IsAlreadyQueued()) {
                if (track.ReasonExpanded()) {
                    track.ReasonExpanded(false);
                } else {
                    track.ReasonExpanded(true);
                }
            }
        },
        
        activate: function (provider, link) {
            return albumRepository.getAlbum(album, provider, link);
        }
    };
});