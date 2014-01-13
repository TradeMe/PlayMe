define(['services/error'],function (error) {
    var getLikes = function (likesObservable, hasMoreObservable) {
        return $.getJSON('api/likes/mylikes',
            {
                start: likesObservable().length,
                take: 50
            },
            function (data) {
                hasMoreObservable(data.HasMorePages);
                ko.utils.arrayPushAll(likesObservable, ko.mapping.fromJS(data).PageData());
            }
        ).error(function () { error.show(error.errors.read); });
    };
    return {
        getLikes: getLikes
    };
});