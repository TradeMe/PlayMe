define(['repositories/searchRepository','repositories/queueRepository'], function (searchRepository, queueRepository) {
    var tabs = {
        artists: 'artists',
        albums: 'albums',
        tracks: 'tracks'
    };
    var results = ko.observable();
    var currentTab = ko.observable(tabs.artists);
    var searchedFor = ko.observable();

    var showTab = function(tab) {
        currentTab(tab);
    };

    var showArtists = function() {
        showTab(tabs.artists);
    };

    var showAlbums = function () {
        showTab(tabs.albums);
    };

    var showTracks = function () {
        showTab(tabs.tracks);
    };

    return {
        tabs: tabs,
        currentTab: currentTab,
        searchedFor: searchedFor,
        results: results,
        showArtists: showArtists,
        showAlbums: showAlbums,
        showTracks: showTracks,
        
        queueTrack: function(track) {
            queueRepository.queueTrack(track);
        },

        activate: function (context) {
            searchedFor(context.searchTerm);
            
            return searchRepository.doSearch(results, context.provider, context.searchTerm).then(function() {
                if (results().PagedArtists.Artists().length > 0) {
                    currentTab(tabs.artists);
                } else if (results().PagedAlbums.Albums().length > 0) {
                    currentTab(tabs.albums);
                } else if (results().PagedTracks.Tracks().length > 0) {
                    currentTab(tabs.tracks);
                } else {
                    currentTab(tabs.artists);
                }
            });
        }
    };
});