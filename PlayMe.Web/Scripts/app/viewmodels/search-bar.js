define(['plugins/router','repositories/searchRepository'], function (router, searchRepository) {
    var providers = ko.observableArray();
    var selectedProvider = ko.observable();
    var searchTerm = ko.observable('');
    
    return {
        searchTerm: searchTerm,
        providers: providers,
        provider: selectedProvider,
        providerName: ko.computed(function() {
            return selectedProvider() ? selectedProvider().Name() : '';
        }),

        providersVisible: ko.computed(function() {
            return providers().length > 1;
        }),
        
        activate: function() {
            return searchRepository.getActiveProviders(providers).then(function() {
                selectedProvider(providers()[0]);
            });
        },

        selectProvider: function(provider) {
            selectedProvider(provider);
        },

        selectedProviderIcon: ko.computed(function () {
            return selectedProvider() ? "/Content/styles/Images/" + selectedProvider().Name() + "logo.png" : "/Content/styles/Images/nullLogo.png";
        }),
        
        doSearch: function() {
            if (searchTerm()) {
                var params = {
                    provider: selectedProvider().Identifier(),
                    searchTerm: searchTerm()
                };
                router.navigate('#search-results?' + $.param(params), { trigger: true });
            }
        }
    };
});