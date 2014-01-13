define(['repositories/userRepository'], function (userRepository) {
    var User = function(username) {
        this.Username = username;
    };
    var newAdminUser = ko.observable(new User(''));
    var adminUsers = ko.observableArray([]);
    return {
        newAdminUser: newAdminUser,
        adminUsers: adminUsers,
        addAdminUser: function(item) {
            if (item.Username == '') {
                return false;
            }
            return userRepository.saveAdminUser(item).then(function() {
                adminUsers.push(item);
                newAdminUser(new User(''));
            });
        },
        removeAdminUser: function(item) {
            return userRepository.removeAdminUser(item).then(function() {
                adminUsers.remove(item);
            });
        },
        activate: function () {
            return userRepository.getAdminUsers(adminUsers);
        }
    };
});