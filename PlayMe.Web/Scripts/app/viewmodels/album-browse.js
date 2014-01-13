define(['repositories/albumRepository', 'repositories/queueRepository'], function (albumRepository, queueRepository) {
    var album = ko.observable();
    
    return {
        album: album,

        queueTrack: function (track) {
            queueRepository.queueTrack(track);
        },
        
        activate: function (provider, link) {
            return albumRepository.getAlbum(album, provider, link);
        }
    };
});