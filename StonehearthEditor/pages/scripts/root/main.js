
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
        return typeof s === 'string' && s.length > 0 && !isNaN(s);
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
                    componentName: "encompassing-group-property",
                    optional: false,
                    children: Ember.A([
                        ComplexProperty.create({
                            name: 'lifetime',
                            componentName: "sub-group-property",
                            optional: true,
                            children: Ember.A([
                                ParameterProperty.create({
                                    name: 'start',
                                    componentName: "level3-parameter-property",
                                    dimension: 'scalar',
                                    timeVarying: false,
                                    optional: true,
                                }),
                            ]),
                        }),
                        ComplexProperty.create({
                            name: 'speed',
                            componentName: "sub-group-property",
                            optional: true,
                            children: Ember.A([
                                ParameterProperty.create({
                                    name: 'start',
                                    componentName: "level3-parameter-property",
                                    dimension: 'scalar',
                                    timeVarying: false,
                                    optional: true,
                                }),
                                ParameterProperty.create({
                                    name: 'over_lifetime',
                                    componentName: "level3-parameter-property",
                                    dimension: 'scalar',
                                    timeVarying: true,
                                    optional: true,
                                }),
                            ]),
                        }),
                        ComplexProperty.create({
                            name: 'acceleration',
                            componentName: "sub-group-property",
                            optional: true,
                            children: Ember.A([
                                ParameterProperty.create({
                                    name: 'over_lifetime_x',
                                    componentName: "level3-parameter-property",
                                    dimension: 'scalar',
                                    timeVarying: true,
                                    optional: true,
                                }),
                                ParameterProperty.create({
                                    name: 'over_lifetime_y',
                                    componentName: "level3-parameter-property",
                                    dimension: 'scalar',
                                    timeVarying: true,
                                    optional: true,
                                }),
                                ParameterProperty.create({
                                    name: 'over_lifetime_z',
                                    componentName: "level3-parameter-property",
                                    dimension: 'scalar',
                                    timeVarying: true,
                                    optional: true,
                                }),
                            ]),
                        }),
                        ComplexProperty.create({
                            name: 'color',
                            componentName: "sub-group-property",
                            optional: true,
                            children: Ember.A([
                                ParameterProperty.create({
                                    name: 'start',
                                    componentName: "level3-parameter-property",
                                    dimension: 'rgba',
                                    timeVarying: false,
                                    optional: true,
                                }),
                                ParameterProperty.create({
                                    name: 'over_lifetime_a',
                                    componentName: "level3-parameter-property",
                                    dimension: 'scalar',
                                    timeVarying: true,
                                    optional: true,
                                }),
                                ParameterProperty.create({
                                    name: 'over_lifetime_r',
                                    componentName: "level3-parameter-property",
                                    dimension: 'scalar',
                                    timeVarying: true,
                                    optional: true,
                                }),
                                ParameterProperty.create({
                                    name: 'over_lifetime_g',
                                    componentName: "level3-parameter-property",
                                    dimension: 'scalar',
                                    timeVarying: true,
                                    optional: true,
                                }),
                                ParameterProperty.create({
                                    name: 'over_lifetime_b',
                                    componentName: "level3-parameter-property",
                                    dimension: 'scalar',
                                    timeVarying: true,
                                    optional: true,
                                }),
                            ]),
                        }),
                        ComplexProperty.create({
                            name: 'scale',
                            componentName: "sub-group-property",
                            optional: true,
                            children: Ember.A([
                                ParameterProperty.create({
                                    name: 'start',
                                    componentName: "level3-parameter-property",
                                    dimension: 'scalar',
                                    timeVarying: false,
                                    optional: true,
                                }),
                                ParameterProperty.create({
                                    name: 'over_lifetime',
                                    componentName: "level3-parameter-property",
                                    dimension: 'scalar',
                                    timeVarying: true,
                                    optional: true,
                                }),
                            ]),
                        }),
                        ComplexProperty.create({
                            name: 'rotation',
                            componentName: "sub-group-property",
                            optional: true,
                            children: Ember.A([
                                ParameterProperty.create({
                                    name: 'over_lifetime_x',
                                    componentName: "level3-parameter-property",
                                    dimension: 'scalar',
                                    timeVarying: true,
                                    optional: true,
                                }),
                                ParameterProperty.create({
                                    name: 'over_lifetime_y',
                                    componentName: "level3-parameter-property",
                                    dimension: 'scalar',
                                    timeVarying: true,
                                    optional: true,
                                }),
                                ParameterProperty.create({
                                    name: 'over_lifetime_z',
                                    componentName: "level3-parameter-property",
                                    dimension: 'scalar',
                                    timeVarying: true,
                                    optional: true,
                                }),
                            ]),
                        }),
                        ComplexProperty.create({
                            name: 'velocity',
                            componentName: "sub-group-property",
                            optional: true,
                            children: Ember.A([
                                ParameterProperty.create({
                                    name: 'over_lifetime_x',
                                    componentName: "level3-parameter-property",
                                    dimension: 'scalar',
                                    timeVarying: true,
                                    optional: true,
                                }),
                                ParameterProperty.create({
                                    name: 'over_lifetime_y',
                                    componentName: "level3-parameter-property",
                                    dimension: 'scalar',
                                    timeVarying: true,
                                    optional: true,
                                }),
                                ParameterProperty.create({
                                    name: 'over_lifetime_z',
                                    componentName: "level3-parameter-property",
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
                    componentName: "encompassing-group-property",
                    optional: false,
                    children: Ember.A([
                        ParameterProperty.create({
                            name: 'rate',
                            componentName: "level2-parameter-property",
                            dimension: 'scalar',
                            timeVarying: false,
                            optional: false,
                        }),
                        ParameterProperty.create({
                            name: 'angle',
                            componentName: "level2-parameter-property",
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

App.IndexController = Ember.Controller.extend({
    actions: {
        preview: function () {
            EffectsJsObject.preview(this.get('model.effectModel').toJson());
        },
        save: function () {
            EffectsJsObject.save(this.get('model.effectModel').toJson());
        },
    },
});

/*App.ShInputComponent = Ember.InputComponent.extend({
    classNames: ['sh-input'],
});

App.ShNumericInputComponent = App.ShInputComponent.extend({
    classNames: ['sh-numeric-input'],
});*/
