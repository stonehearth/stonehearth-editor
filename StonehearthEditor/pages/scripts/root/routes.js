App.IndexRoute = Ember.Route.extend({
    model: function () {
        var effectKind = CsApi.effectKind;
        var json = CsApi.json;
        var effectModel = EffectKinds[effectKind]();
        effectModel.fromJson(json);
        return {
            effectModel: effectModel,
        };
    },
});

App.ApplicationController = Ember.Controller.extend();

