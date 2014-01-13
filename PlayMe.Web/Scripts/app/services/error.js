define(['services/messenger'], function(messenger) {
    var errors = {
        connection: 'An error occurred while requesting data from the server'
    };

    return {
        errors: errors,
        show: function(error) {
            messenger.show(error, messenger.types.error);
        }
    };
});