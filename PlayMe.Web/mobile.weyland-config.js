exports.config = function(weyland) {
    weyland.build('main.mobile')
        .task.jshint({
            include: 'Scripts/app/viewmodels/**/*.js'
        })
        .task.rjs({
            include: ['Scripts/app/main.mobile.js', 'Scripts/app/viewmodels/*.js', 'Scripts/app/views.mobile/**/*.html', 'Scripts/durandal/**/*.js'],
            loaderPluginExtensionMaps: {
                '.html': 'text'
            },
            rjs: {
                name: '../almond-custom', //to deploy with require.js, use the build's name here instead
                insertRequire: ['main.mobile'], //not needed for require
                baseUrl: 'Scripts/app',
                wrap: true, //not needed for require
                paths: {
                    'text': '../text',
                    'durandal': '../durandal',
                    'plugins': '../durandal/plugins',
                    'transitions': '../durandal/transitions',
                    'knockout': 'empty:',
                    'bootstrap': 'empty:',
                    'jquery': 'empty:'
                },
                inlineText: true,
                optimize: 'none',
                pragmas: {
                    build: true
                },
                stubModules: ['text'],
                keepBuildDir: true,
                out: 'Scripts/app/main.mobile-built.js'
            }            
        });    
}

