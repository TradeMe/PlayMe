define(['plugins/router','durandal/app'], function (router, app) {

    return {
        authorizedRoutes: ko.computed(function() {
            return router.navigationModel().filter(function (r) {
                 return (r.settings && r.settings.admin) ? app.isAdmin() : true;
            });
        }),
        router: router,
        activate: function () {
            return router.map([
                { route: ['now-playing', ''], moduleId: 'viewmodels/now-playing', title: 'Now Playing', nav: true },
                { route: 'queue', moduleId: 'viewmodels/queue', title: 'Queue', nav: true },
                { route: 'history(/:filter)', moduleId: 'viewmodels/history', title: 'History', hash: '#history', nav: true },
                { route: 'likes', moduleId: 'viewmodels/likes', title: 'Likes', nav: true },                
                { route: 'admin*details', moduleId: 'viewmodels/admin', title: 'Admin', hash: '#admin', nav: true, settings: { admin: true } },
                { route: 'artists/:provider/:link', moduleId: 'viewmodels/artist-browse', title: 'Browse Artist', nav: false },
                { route: 'albums/:provider/:link', moduleId: 'viewmodels/album-browse', title: 'Browse Album', nav: false },
                { route: 'search-results', moduleId: 'viewmodels/search-results', title: 'Search Results', nav: false }               
            ]).buildNavigationModel()
              .activate();
        }
    };
});