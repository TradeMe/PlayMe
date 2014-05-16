define(['services/error'],function (error) {
    var getAlbum = function (albumObservable, provider, link) {
        return $.getJSON('api/browse/album/' + provider + '/' + link,
            function (data) {
                var results = ko.mapping.fromJS(data);
                ko.utils.arrayForEach(results.Tracks(), function (item) {
                    var otherStuff = { ReasonExpanded: ko.observable(false) };
                    $.extend(item, otherStuff);
                });
                albumObservable(results);
            }
        ).error(function () { error.show(error.errors.read); });
    };
    return {
        getAlbum: getAlbum
    };
});