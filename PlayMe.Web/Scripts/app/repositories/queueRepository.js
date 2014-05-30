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
            var tr = new Object();
            tr.id = encodeURIComponent(track.Link());
            tr.provider = track.MusicProvider.Identifier();
            tr.reason = track.Reason();
            return $.post(
                'api/Queue/Enqueue',
                tr,
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