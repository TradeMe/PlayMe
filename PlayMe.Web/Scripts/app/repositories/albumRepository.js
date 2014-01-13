define(['services/error'],function (error) {
    var getAlbum = function (albumObservable, provider, link) {
        return $.getJSON('api/browse/album/' + provider + '/' + link,
            function(data) {
                albumObservable(ko.mapping.fromJS(data));
            }
        ).error(function () { error.show(error.errors.read); });
    };
    return {
        getAlbum: getAlbum
    };
});