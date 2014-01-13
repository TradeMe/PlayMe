define(['services/error'], function (error) {
    var getLogEntries = function (logObservable, hasMoreObservable, sortDirection) {
        var params = {
            direction: sortDirection(),
            start: logObservable().length,
            take: 50
        };

        return $.getJSON(
            'api/admin/getLogEntries',
            params,
            function (data) {
                hasMoreObservable(data.HasMorePages);
                ko.utils.arrayPushAll(logObservable, ko.mapping.fromJS(data).PageData());
            }
        )
        .error(function () { error.show(error.errors.read); });
    };
    return {
        getLogEntries: getLogEntries
    };
});