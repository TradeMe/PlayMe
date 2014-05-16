define(['services/error'],function (error) {
    var getLikes = function (likesObservable, hasMoreObservable) {
        return $.getJSON('api/likes/mylikes',
            {
                start: likesObservable().length,
                take: 50
            },
            function (data) {
                hasMoreObservable(data.HasMorePages);

                var results = ko.mapping.fromJS(data).PageData();
                ko.utils.arrayForEach(results, function (item) {
                    var otherStuff = { ReasonExpanded: ko.observable(false) };
                    $.extend(item, otherStuff);
                });
                ko.utils.arrayPushAll(likesObservable, results);
            }
        ).error(function () { error.show(error.errors.read); });
    };
    return {
        getLikes: getLikes
    };
});