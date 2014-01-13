define(function() {
    var types = {
        error: 'error',
        warn: 'warn',
        info: 'info'
    };
    
    return {
        types: types,
        show: function(message, type) {
            $.notify(message, { className: type, globalPosition: 'bottom right'});
        }
    };
})