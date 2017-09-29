EffectKinds = {
    cubemitter: function () {
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
                    name: "emission",
                    componentName: "encompassing-group-property",
                    optional: false,
                    children: Ember.A([
                        ParameterProperty.create({
                            name: 'rate',
                            componentName: "level2-parameter-property",
                            dimension: 'scalar',
                            timeVarying: true,
                            optional: false,
                        }),
                        BurstProperty.create({
                            name: 'bursts',
                            componentName: "burst-parameter-property",
                            dimension: 'scalar',
                            timeVarying: true,
                            optional: false,
                        }),
                        ParameterProperty.create({
                            name: 'angle',
                            componentName: "level2-parameter-property",
                            dimension: 'scalar',
                            timeVarying: true,
                            optional: false,
                        }),
                        OriginProperty.create({
                            name: "origin",
                            componentName: "origin-parameter-property",
                            dimension: 'scalar',
                            timeVarying: false,
                            optional: false,
                        }),
                        ParameterProperty.create({
                            name: "translation_x",
                            componentName: "level2-parameter-property",
                            dimension: 'scalar',
                            timeVarying: true,
                            optional: true,
                        }),
                        ParameterProperty.create({
                            name: "translation_y",
                            componentName: "level2-parameter-property",
                            dimension: 'scalar',
                            timeVarying: true,
                            optional: true,
                        }),
                        ParameterProperty.create({
                            name: "translation_z",
                            componentName: "level2-parameter-property",
                            dimension: 'scalar',
                            timeVarying: true,
                            optional: true,
                        }),
                        ParameterProperty.create({
                            name: "rotation_x",
                            componentName: "level2-parameter-property",
                            dimension: 'scalar',
                            timeVarying: true,
                            optional: true,
                        }),
                        ParameterProperty.create({
                            name: "rotation_y",
                            componentName: "level2-parameter-property",
                            dimension: 'scalar',
                            timeVarying: true,
                            optional: true,
                        }),
                        ParameterProperty.create({
                            name: "rotation_z",
                            componentName: "level2-parameter-property",
                            dimension: 'scalar',
                            timeVarying: true,
                            optional: true,
                        }),
                        ParameterProperty.create({
                            name: "scale_x",
                            componentName: "level2-parameter-property",
                            dimension: 'scalar',
                            timeVarying: true,
                            optional: true,
                        }),
                        ParameterProperty.create({
                            name: "scale_y",
                            componentName: "level2-parameter-property",
                            dimension: 'scalar',
                            timeVarying: true,
                            optional: true,
                        }),
                        ParameterProperty.create({
                            name: "scale_z",
                            componentName: "level2-parameter-property",
                            dimension: 'scalar',
                            timeVarying: true,
                            optional: true,
                        }),
                    ]),
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
                                ParameterProperty.create({
                                    name: 'start_x',
                                    componentName: "level3-parameter-property",
                                    dimension: 'scalar',
                                    timeVarying: false,
                                    optional: true,
                                }),
                                ParameterProperty.create({
                                    name: 'start_y',
                                    componentName: "level3-parameter-property",
                                    dimension: 'scalar',
                                    timeVarying: false,
                                    optional: true,
                                }),
                                ParameterProperty.create({
                                    name: 'start_z',
                                    componentName: "level3-parameter-property",
                                    dimension: 'scalar',
                                    timeVarying: false,
                                    optional: true,
                                }),
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
                                    name: 'over_lifetime',
                                    componentName: "level3-parameter-property",
                                    dimension: 'rgb',
                                    timeVarying: true,
                                    optional: true,
                                }),
                                ParameterProperty.create({
                                    name: 'over_lifetime_a',
                                    componentName: "level3-parameter-property",
                                    dimension: 'scalar',
                                    timeVarying: true,
                                    optional: true,
                                }),
                            ]),
                        }),
                    ]),
                }),
            ]),
        });
    },
    animatedlight: function () {
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

                BooleanProperty.create({
                    name: "loops",
                }),

                ComplexProperty.create({
                    name: "intensity",
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
                    name: "radius",
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
                    name: "inner_radius",
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
                    name: 'color',
                    componentName: "sub-group-property",
                    optional: true,
                    children: Ember.A([
                        ParameterProperty.create({
                            name: 'start',
                            componentName: "level3-parameter-property",
                            dimension: 'rgb',
                            timeVarying: false,
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
            ]),
        });
    },
}