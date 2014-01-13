define(['repositories/artistRepository'], function (artistRepository) {

    var tabs = {
        albums: "albums",
        similarArtists: "similarArtists"
    };

    var currentTab = ko.observable(tabs.albums);

    var artist = ko.observable();

    var showAlbums = function () {
        currentTab(tabs.albums);
    };

    var showSimilarArtists = function () {
        currentTab(tabs.similarArtists);
    };
    
    return {
        tabs: tabs,
        currentTab: currentTab,
        artist: artist,
        showAlbums: showAlbums,
        showSimilarArtists: showSimilarArtists,
        activate: function(provider, link) {
            return artistRepository.getArtist(artist, provider, link);
        }
    };
});
