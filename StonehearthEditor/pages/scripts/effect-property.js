EffectProperty = Ember.Object.extend({
    name: null,
    isMissing: function() {
        throw "NotImplemented"
    }.property(),
    isValid: function() {
        throw "NotImplemented"
    }.property(),
    toJson: function () {
        throw "NotImplemented"
    },
    fromJson: function (json) {
        throw "NotImplemented"
    }
});