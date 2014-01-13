exports.config = function(weyland) {
    weyland.build('main')
        .task.jshint({
            include: 'Scripts/app/viewmodels/**/*.js'
        })
        .task.rjs({
            include: ['Scripts/app/main.js', 'Scripts/app/viewmodels/*.js', 'Scripts/app/views/**/*.html', 'Scripts/durandal/**/*.js'],
            loaderPluginExtensionMaps: {
                '.html': 'text'
            },
            rjs: {
                name: '../almond-custom', //to deploy with require.js, use the build's name here instead
                insertRequire: ['main'], //not needed for require
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
                out: 'Scripts/app/main-built.js'
            }            
        });    
}

