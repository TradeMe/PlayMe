define(['services/error'],function (error) {
    return {        
        isAdmin: function (observable) {
            //we don't care about an error
            return $.getJSON(
                'api/admin/isUserAdmin',
                function(data) {
                    observable(data);
                }
            );
        },
        getAdminUsers : function (adminUsersObservable) {
            return $.getJSON(
                'api/admin/GetAdminUsers',
                function (data) {
                    adminUsersObservable(data);
                }
            ).error(function () { error.show(error.errors.read); });
        },
        saveAdminUser: function (item) {
            return $.ajax('api/admin/AddAdminUser',
                {
                    data: ko.toJSON(item),
                    type: 'post',
                    contentType: 'application/json'
                }
            ).error(function () { error.show(error.errors.save); });
        },
        removeAdminUser: function(item) {
            return $.ajax('api/admin/RemoveAdminUser/', {
                data: ko.toJSON(item),
                type: 'post',
                contentType: 'application/json'
                }
            ).error(function () { error.show(error.errors.save); });
        }
    };
});