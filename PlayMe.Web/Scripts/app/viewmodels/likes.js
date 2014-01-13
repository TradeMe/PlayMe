define(['repositories/likesRepository', 'repositories/queueRepository'], function (likesRepository, queueRepository) {
    var likes = ko.observableArray([]);
    var hasMorePages = ko.observable(false);
    var getLikes = function (clear) {
        if (clear) {
            likes([]);
        }
        return likesRepository.getLikes(likes, hasMorePages);
    };
    return {
        likes: likes,

        queueTrack: function (track) {
            queueRepository.queueTrack(track);
        },

        getMoreLikes: function () {
            return getLikes(false);
        },

        showMore: ko.computed(function () {
            return (likes.length > 0 && hasMorePages());
        }),
        
        activate: function () {
            return getLikes(true);
        }
    };
});
