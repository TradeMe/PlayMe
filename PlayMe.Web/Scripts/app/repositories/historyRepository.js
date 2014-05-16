define(['services/error'],function (error) {
    var getHistory = function (historyObservable, hasMoreObservable, historyFilter) {
        return $.getJSON('api/history',
            {
                filter: historyFilter,
                start: historyObservable().length,
                take: 50
            },
            function (data) {
                hasMoreObservable(data.HasMorePages);

                var results = ko.mapping.fromJS(data).PageData();
                ko.utils.arrayForEach(results, function (item) {
                    var otherStuff = { ReasonExpanded: ko.observable(false) };
                    $.extend(item.Track, otherStuff);
                });
                ko.utils.arrayPushAll(historyObservable, results);
            }
        ).error(function () { error.show(error.errors.read); });
    };
    return {
        getHistory: getHistory   
    };
});