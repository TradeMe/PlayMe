var gulp = require('gulp');
var durandal = require('gulp-durandal');

gulp.task('default', function() {
	durandal({
            baseDir: 'Scripts/app',
            main: 'main.js',
            output: 'main-built.js',
            almond: true,
            minify: true,
            verbose: true
        })
	.pipe(gulp.dest('Scripts/app'));
	durandal({
            baseDir: 'Scripts/app',
            main: 'main.mobile.js',
            output: 'main.mobile-built.js',
            almond: true,
            minify: true,
            verbose: true
        })
	.pipe(gulp.dest('Scripts/app'));
});
