define(['services/error'],function (error) {
    var getArtist = function (artistObservable, provider, link) {
        return $.getJSON('api/browse/artist/' + provider + '/' + link,
            function(data) {
                artistObservable(ko.mapping.fromJS(data));                
            }
        ).error(function () { error.show(error.errors.read); });
    };
    return {
        getArtist: getArtist
    };
});