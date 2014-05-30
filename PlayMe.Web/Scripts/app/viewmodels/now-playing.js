define(['durandal/app','repositories/queueRepository'], function (app, queueRepository) {
    var tabs = {
        playingSoon: 'playingSoon',
        recentlyPlayed: 'recentlyPlayed'
    };
    var nowPlaying = ko.observable(null);
    var playingSoon = ko.observableArray();
    var recentlyPlayed = ko.observableArray();

    var currentTab = ko.observable(tabs.playingSoon);

    var currentTime = ko.observable();
    var currentVolume = ko.observable();

    var positionAsMilliseconds = ko.computed(function () {
        if (!nowPlaying()) return 0;
        var startAsMs = Date.parse(nowPlaying().StartedPlayingDateTime());
        var currentAsMs = currentTime();
        var positionAsMs = currentAsMs - (startAsMs + nowPlaying().PausedDurationAsMilliseconds());

        //position can't be greater than the duration
        if (positionAsMs > nowPlaying().Track.DurationMilliseconds()) positionAsMs = nowPlaying().Track.DurationMilliseconds();
        return positionAsMs;
    });

    var setCurrentTime = function () {
        if (currentTime() == null || nowPlaying() != null && !nowPlaying().IsPaused())
            currentTime(new Date().getTime());
    };

    var hub = $.connection.queueHub;

    var init = function () {
        //Todo: wrap in a single method on the hub
        hub.server.getCurrentTrack();
        hub.server.getPlayingSoon();
        hub.server.getRecentlyPlayed();
        hub.server.getCurrentVolume();
    };

    //signalr callback when playing track changes
    hub.client.updateCurrentTrack = function (data) {
        //We may get nothing (null) back from the server if the track hasn't started playing
        if (data) {
            nowPlaying(ko.mapping.fromJS(data));
            setCurrentTime();
        }
    };

    hub.client.updateCurrentVolume = function (data) {
        //We may get nothing (null) back from the server if the track hasn't started playing
        if (data) {
            currentVolume(data);
        }
    };

    //signalr callback when track queued/dequeued
    hub.client.updatePlayingSoon = function (data) {
        var mappedData = ko.mapping.fromJS(data)();
        ko.utils.arrayForEach(mappedData, function (item) {
            var otherStuff = { queueActionsShown: ko.observable(false) };
            $.extend(item, otherStuff);
        });

        playingSoon(mappedData);
    };

    hub.client.updateRecentlyPlayed = function (data) {
        var results = ko.mapping.fromJS(data).PageData();
        ko.utils.arrayForEach(results, function (item) {
            var otherStuff = { ReasonExpanded: ko.observable(false) };
            $.extend(item.Track, otherStuff);
        });

        recentlyPlayed(results);
    };

    return {
        tabs: tabs,
        isAdmin: app.isAdmin,
        isSuperAdmin: app.isSuperAdmin,
        nowPlaying: nowPlaying,
        playingSoon: playingSoon,
        recentlyPlayed: recentlyPlayed,
        currentTab: currentTab,
        currentTime: currentTime,
        currentVolume: currentVolume,

        showPlayingSoon: function () {
            currentTab(tabs.playingSoon);
        },

        showRecentlyPlayed: function () {
            currentTab(tabs.recentlyPlayed);
        },

        vetoTrack: function (track) {
            hub.server.vetoTrack(track.Id());
        },

        likeTrack: function (track) {
            hub.server.likeTrack(track.Id());
        },

        nextTrack: function (track) {
            hub.server.nextTrack(track.Id());
        },

        forgetTrack: function (track) {
            hub.server.forgetTrack(track.Id());
        },

        pausePlayTrack: function () {
            if (nowPlaying()) {
                if (!nowPlaying().IsPaused()) {
                    hub.server.pauseTrack();
                } else {
                    hub.server.resumeTrack();
                }
            }
        },

        increaseVolume: function () {
            hub.server.increaseVolume();
        },

        decreaseVolume: function () {
            hub.server.decreaseVolume();
        },

        positionAsMilliseconds: positionAsMilliseconds,

        positionAsPercent: ko.computed(function () {
            if (nowPlaying() == null) return 0;

            var durationAsMs = nowPlaying().Track.DurationMilliseconds();
            return (positionAsMilliseconds() / durationAsMs) * 100;

        }),
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
        showQueueActions: function (item) {
            if (!item.StartedPlayingDateTime()) {
                item.queueActionsShown(true);
            }
        },

        hideQueueActions: function (item) {
            if (!item.StartedPlayingDateTime()) {
                item.queueActionsShown(false);
            }
        },
        activate: function () {
            if ($.connection.hub.state === $.signalR.connectionState.disconnected) {
                //Set the current time every 1/2 second.
                setInterval(function () { setCurrentTime(); }, 500);
                
                //Exclude forever frame (on ie) to get around ko.mapping issue
                return $.connection.hub.start({ transport: ['webSockets', 'serverSentEvents', 'longPolling'] })
                    .done(function () {
                        init();
                    });
            }
        }
    };
});
