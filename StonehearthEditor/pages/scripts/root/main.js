
App = Ember.Application.create({
});

Utils = {
    isUndefinedOrTypeOf: function (type, obj) {
        return typeof (obj) === type || typeof (obj) === undefined;
    },
    assert: function (condition, message) {
        if (!condition) {
            throw "AssertionError: " + (message || "");
        }
    }

};

EffectKinds = {
    cubeEmitter: function () {
        return ComplexProperty.create({
            name: "",
            optional: false,
            children: Ember.A([
                StringProperty.create({
                    name: "name",
                }),
            ]),
        });
    },
}

CsApi = {
    onSelectionChanged: function () {

    },
};

IndexController = Ember.Controller.extend({
    actions: {
    },
});

App.EffectPropertyComponent = Ember.Component.extend({
    templateName: function () {
        var model = this.get('model');
        if (model instanceof StringProperty) {
            return "string-property-template";
        } else if (model instanceof ComplexProperty) {
            return "complex-property-template";
        } else {
            Utils.assert(false, "Invalid effect property " + this);
        }
    }.property(),
});
