define(['repositories/logRepository'], function (logRepository) {

    var logEntries = ko.observableArray([]);    
    var sortDirection = ko.observable('Descending');
    var hasMorePages = ko.observable(false);
    
    var toggleDirection = function () {
        if (sortDirection() == 'Ascending') {
            sortDirection('Descending');
        } else {
            sortDirection('Ascending');
        }
        getLogEntries(true);
    };
    
    var getLogEntries = function (clear) {
        if (clear) {
            logEntries([]);
        }
        return logRepository.getLogEntries(logEntries, hasMorePages, sortDirection);
    };
    return {
        logEntries: logEntries,
        sortDirection: sortDirection,
        toggleDirection: toggleDirection,

        getMoreLogEntries: function () {
            return getLogEntries(false);
        },

        showMore: ko.computed(function () {
            return (logEntries().length > 0 && hasMorePages());
        }),
        
        activate: function () {
            return getLogEntries(true);
        }
    };
});