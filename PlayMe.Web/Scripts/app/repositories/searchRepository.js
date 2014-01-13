define(['services/error'],function (error) {
    var doSearch = function (resultsObservable, provider, searchTerm) {
        var params = {
            provider: provider,
            searchTerm: searchTerm
        };
        return $.getJSON(
            'api/search',
            params,
            function(data) {
                resultsObservable(ko.mapping.fromJS(data));
            }
        ).error(function () { error.show(error.errors.read); });
    };
    var getActiveProviders = function (providersObservable) {
        return $.getJSON(
            'api/search/activeMusicProviders',
            function (data) {
                providersObservable(ko.mapping.fromJS(data)());
            }
        ).error(function () { error.show(error.errors.connection); });
    };
    return {
        doSearch: doSearch,
        getActiveProviders: getActiveProviders
    };    
});
