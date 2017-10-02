App = Ember.Application.create({
});

CsApi = {};

App.BlockPropertyComponent = Ember.Component.extend({
    actions: {
        toggleMissing: function () {
            this.set('model.isMissing', !this.get('model.isMissing'));
        },
    },
});

App.SubGroupPropertyComponent = Ember.Component.extend({
    classNames: ['sub-group-property'],
    actions: {
        toggleMissing: function () {
            this.set('model.isMissing', !this.get('model.isMissing'));
        },
    },
});

App.Level3ParameterPropertyComponent = Ember.Component.extend({
    classNames: ['level3-parameter-property'],
    actions: {
        toggleMissing: function () {
            this.set('model.isMissing', !this.get('model.isMissing'));
        },
    },
});

App.Level2ParameterPropertyComponent = Ember.Component.extend({
    classNames: ['level2-parameter-property'],
    actions: {
        toggleMissing: function () {
            this.set('model.isMissing', !this.get('model.isMissing'));
        },
    },
});

App.Level2ParameterPropertyEmissionComponent = Ember.Component.extend({
    classNames: ['level2-parameter-property-emission'],
    actions: {
        toggleMissing: function () {
            this.set('model.isMissing', !this.get('model.isMissing'));
        },
    },
});

App.OriginParameterPropertyComponent = Ember.Component.extend({
    classNames: ['origin-parameter-property'],
    actions: {
        toggleMissing: function () {
            this.set('model.isMissing', !this.get('model.isMissing'));
        },
    },
});

App.EqHelper = function (a) {
    return a[0] === a[1];
};

App.TostrHelper = function (a) {
    return a[0].toString();
};

App.WarningIconComponent = Ember.Component.extend({
    tagName: 'span',
});

App.CurveXComponent = Ember.Component.extend({
    actions: {
        add: function () {
            this.model.points.pushObject(Point.create({}));
        },
        addAbove: function (index) {
            this.model.points.insertAt(index, Point.create({}));
        },
        delete: function (index) {
            this.model.points.removeAt(index);
        },
    },
});

App.CurveRgbComponent = Ember.Component.extend({
    actions: {
        add: function () {
            this.model.pointsRGB.pushObject(PointRgb.create({}));
        },
        addAbove: function (index) {
            this.model.pointsRGB.insertAt(index, PointRgb.create({}));
        },
        delete: function (index) {
            this.model.pointsRGB.removeAt(index);
        },
    },
});

App.BurstParameterPropertyComponent = Ember.Component.extend({
    actions: {
        add: function () {
            this.model.points.pushObject(Point_Burst.create({}));
        },
        addAbove: function (index) {
            this.model.points.insertAt(index, Point_Burst.create({}));
        },
        delete: function (index) {
            this.model.points.removeAt(index);
        },
    },
});

App.IndexController = Ember.Controller.extend({
    init: function () {
        this._super();
        var self = this;
        $(window).bind('keydown', function (event) {
            if (event.ctrlKey || event.metaKey) {
                var charCode = String.fromCharCode(event.which).toLowerCase();
                if (charCode === "s") {
                    self.save();
                }
            }
        });
    },
    save: function () {
        var jsonObj = this.get('model.effectModel').toJson();
        var json = Utils.formatEffectJson(jsonObj);
        EffectsJsObject.save(json);
    },
    actions: {
        preview: function () {
            EffectsJsObject.preview(this.get('model.effectModel').toJson());
        },
        save: function () {
            this.save();
        },
    },
});
