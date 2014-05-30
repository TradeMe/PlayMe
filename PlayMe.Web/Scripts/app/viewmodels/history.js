define(['repositories/historyRepository', 'repositories/queueRepository'], function (historyRepository, queueRepository) {
    var history = ko.observableArray([]);
    //filters
    var historyFilter = ko.observable();
    var hasMorePages = ko.observable(false);
    var getHistory = function (clear) {
        if (clear) {
            history([]);
        }
        return historyRepository.getHistory(history, hasMorePages, historyFilter());
    };
    
    return {
        history: history,
        filter: historyFilter,
        showMore: ko.computed(function() {
            return (history().length > 0 && hasMorePages());
        }),
        getMoreHistory: function () {
            return getHistory(false);
        },
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
        activate: function (filter) {
            historyFilter(filter || 'all');
            return getHistory(true);
        }
    };
});