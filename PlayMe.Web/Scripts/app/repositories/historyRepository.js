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
                ko.utils.arrayPushAll(historyObservable, ko.mapping.fromJS(data).PageData());                
            }
        ).error(function () { error.show(error.errors.read); });
    };
    return {
        getHistory: getHistory   
    };
});