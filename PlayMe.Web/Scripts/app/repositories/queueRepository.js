define(['services/error'],function (error) {
    var getQueue = function (queueObservable) {
        return $.getJSON(
            'api/queue',
            function(data) {
                queueObservable(ko.mapping.fromJS(data)());
            }
        ).error(function() { error.show(error.errors.read); });
    };

    var queueTrack = function (track) {        
        if (track.IsAlreadyQueued() == false) {
            return $.post(
                'api/Queue/Enqueue/' + track.MusicProvider.Identifier() + '/' + encodeURIComponent(track.Link()),
                function (data) {
                    if (data != '') {
                        track.IsAlreadyQueued(false);
                        error.show(data);                        

                    } else {
                        track.IsAlreadyQueued(true);
                    }
                }
            ).error(function () { error.show(error.errors.read); });
        }
    };

    return {
        getQueue: getQueue,
        queueTrack: queueTrack
    };
    
});