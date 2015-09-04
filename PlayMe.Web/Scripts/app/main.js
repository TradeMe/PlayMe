requirejs.config({
    paths: {
        'text': '../text',
        'durandal': '../durandal',
        'plugins': '../durandal/plugins',
        'transitions': '../durandal/transitions'
    }
});

define('jquery', function () { return jQuery; });
define('knockout', ko);
define(['durandal/system', 'durandal/app', 'durandal/viewLocator', 'repositories/userRepository'], function (system, app, viewLocator, userRepository) {
    //>>excludeStart("build", true);
    system.debug(true);
    //>>excludeEnd("build");

    app.title = 'Play Me';

    app.configurePlugins({
        router: true,
        dialog: true,
        widget: true
    });
    
    app.isAdmin = ko.observable(false);
    userRepository.isAdmin(app.isAdmin);

    app.isSuperAdmin = ko.observable(false);
    userRepository.isSuperAdmin(app.isSuperAdmin);

    app.start().then(function () {
        viewLocator.useConvention();
        app.setRoot('viewmodels/shell', 'entrance');
    });
});