define(['repositories/queueRepository'], function(repository) {
    var queue = ko.observableArray();
    return {
        queue: queue,
        activate: function () {            
            return repository.getQueue(queue);
        }
    };
});