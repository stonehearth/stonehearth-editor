
App = Ember.Application.create({
});

Utils = {
    isUndefinedOrTypeOf: function (type, obj) {
        return typeof (obj) === type || typeof (obj) === 'undefined';
    },
    assert: function (condition, message) {
        if (!condition) {
            console.trace("Error at");
            throw "AssertionError: " + (message || "");
        }
    },
    isNumber: function (s) {
        return !isNaN(s);
    },

};

EffectKinds = {
    cubeEmitter: function () {
        return ComplexProperty.create({
            name: null,
            optional: false,
            children: Ember.A([
                StringProperty.create({
                    name: "name",
                }),

                IntProperty.create({
                    name: "duration",
                }),

                ComplexProperty.create({
                    name: "particle",
                    optional: false,
                    children: Ember.A([
                        ComplexProperty.create({
                            name: 'lifetime',
                            optional: true,
                            children: Ember.A([
                                ParameterProperty.create({
                                    name: 'start',
                                    dimension: 'scalar',
                                    timeVarying: false,
                                    optional: true,
                                }),
                            ]),
                        }),
                        ComplexProperty.create({
                            name: 'speed',
                            optional: true,
                            children: Ember.A([
                                ParameterProperty.create({
                                    name: 'start',
                                    dimension: 'scalar',
                                    timeVarying: false,
                                    optional: true,
                                }),
                                ParameterProperty.create({
                                    name: 'over_lifetime',
                                    dimension: 'scalar',
                                    timeVarying: true,
                                    optional: true,
                                }),
                            ]),
                        }),
                        ComplexProperty.create({
                            name: 'acceleration',
                            optional: true,
                            children: Ember.A([
                                ParameterProperty.create({
                                    name: 'over_lifetime_x',
                                    dimension: 'scalar',
                                    timeVarying: true,
                                    optional: true,
                                }),
                                ParameterProperty.create({
                                    name: 'over_lifetime_y',
                                    dimension: 'scalar',
                                    timeVarying: true,
                                    optional: true,
                                }),
                                ParameterProperty.create({
                                    name: 'over_lifetime_z',
                                    dimension: 'scalar',
                                    timeVarying: true,
                                    optional: true,
                                }),
                            ]),
                        }),
                        ComplexProperty.create({
                            name: 'color',
                            optional: true,
                            children: Ember.A([
                                ParameterProperty.create({
                                    name: 'start',
                                    dimension: 'rgba',
                                    timeVarying: false,
                                    optional: true,
                                }),
                                ParameterProperty.create({
                                    name: 'over_lifetime_a',
                                    dimension: 'scalar',
                                    timeVarying: true,
                                    optional: true,
                                }),
                                ParameterProperty.create({
                                    name: 'over_lifetime_r',
                                    dimension: 'scalar',
                                    timeVarying: true,
                                    optional: true,
                                }),
                                ParameterProperty.create({
                                    name: 'over_lifetime_g',
                                    dimension: 'scalar',
                                    timeVarying: true,
                                    optional: true,
                                }),
                                ParameterProperty.create({
                                    name: 'over_lifetime_b',
                                    dimension: 'scalar',
                                    timeVarying: true,
                                    optional: true,
                                }),
                            ]),
                        }),
                        ComplexProperty.create({
                            name: 'scale',
                            optional: true,
                            children: Ember.A([
                                ParameterProperty.create({
                                    name: 'start',
                                    dimension: 'scalar',
                                    timeVarying: false,
                                    optional: true,
                                }),
                                ParameterProperty.create({
                                    name: 'over_lifetime',
                                    dimension: 'scalar',
                                    timeVarying: true,
                                    optional: true,
                                }),
                            ]),
                        }),
                        ComplexProperty.create({
                            name: 'rotation',
                            optional: true,
                            children: Ember.A([
                                ParameterProperty.create({
                                    name: 'over_lifetime_x',
                                    dimension: 'scalar',
                                    timeVarying: true,
                                    optional: true,
                                }),
                                ParameterProperty.create({
                                    name: 'over_lifetime_y',
                                    dimension: 'scalar',
                                    timeVarying: true,
                                    optional: true,
                                }),
                                ParameterProperty.create({
                                    name: 'over_lifetime_z',
                                    dimension: 'scalar',
                                    timeVarying: true,
                                    optional: true,
                                }),
                            ]),
                        }),
                        ComplexProperty.create({
                            name: 'velocity',
                            optional: true,
                            children: Ember.A([
                                ParameterProperty.create({
                                    name: 'over_lifetime_x',
                                    dimension: 'scalar',
                                    timeVarying: true,
                                    optional: true,
                                }),
                                ParameterProperty.create({
                                    name: 'over_lifetime_y',
                                    dimension: 'scalar',
                                    timeVarying: true,
                                    optional: true,
                                }),
                                ParameterProperty.create({
                                    name: 'over_lifetime_z',
                                    dimension: 'scalar',
                                    timeVarying: true,
                                    optional: true,
                                }),
                            ]),
                        }),
                    ]),
                }),

                ComplexProperty.create({
                    name: "emission",
                    optional: false,
                    children: Ember.A([
                        ParameterProperty.create({
                            name: 'rate',
                            dimension: 'scalar',
                            timeVarying: false,
                            optional: false,
                        }),
                        ParameterProperty.create({
                            name: 'angle',
                            dimension: 'scalar',
                            timeVarying: false,
                            optional: false,
                        }),
                        OriginProperty.create({
                            name: "origin",
                        }),
                    ]),
                }),
            ]),
        });
    },
}

CsApi = {
    onSelectionChanged: function () {

    },
};

App.OriginPropertyComponent = Ember.Component.extend({
    surfaceOptions: ['POINT', 'RECTANGLE'],
});

App.BlockPropertyComponent = Ember.Component.extend({
    actions: {
        toggleMissing: function () {
            this.set('model.isMissing', !this.get('model.isMissing'));
        },
    },
});

App.EqHelper = function (a) {
    return a[0] === a[1];
};

App.WarningIconComponent = Ember.Component.extend({
    tagName: 'span',
});
