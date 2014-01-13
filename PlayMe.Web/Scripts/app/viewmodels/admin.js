define(['plugins/router'], function (router) {
    var childRouter = router.createChildRouter().map([
            { route: ['admin/users', 'admin'], moduleId: 'viewmodels/admin-users', title: 'Users', nav: true },
            { route: 'admin/log', moduleId: 'viewmodels/admin-log', title: 'Log', nav: true}
    ]).buildNavigationModel();
    return {
        router: childRouter
    };
});