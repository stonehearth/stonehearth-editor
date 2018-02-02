OriginSurface = Ember.Object.extend({
    componentName: null,
    fromJson: function (json) {
        throw "NotImplemented";
    },
    toJson: function () {
        throw "NotImplemented";
    },
    isValid: function () {
        throw "NotImplemented";
    },
});

ConstantPointOriginSurface = OriginSurface.extend({
    componentName: 'point-parameter-property',
    surface: null, // string or null
    isMissing: null,
    toJson: function () {
        return;
    },
    fromJson: function (json) {
        Utils.assert(Utils.isUndefinedOrTypeOf("object", json));
        if (json === undefined) {
            return;
        }
        this.set('surface', Utils.getEffectValueOrDefault(json, 'surface', 'POINT'));
    },
});

ConstantRectangleOriginSurface = OriginSurface.extend({
    componentName: 'rectangle-scalar-parameter',
    width: '1',
    length: '1',
    innerWidth: '0',
    innerLength: '0',
    isValid: Ember.computed('width','length','innerWidth','innerLength', function () {
        return Utils.isNumber(this.width) && Utils.isNumber(this.length) && 
            Utils.isNumber(this.innerWidth) && Utils.isNumber(this.innerLength);
    }),
    invalidValueMessage: Ember.computed('width','length','innerWidth','innerLength', function () {
        if (!Utils.isNumber(this.get('width'))) {
            return "Invalid width.";
        }
        if (!Utils.isNumber(this.get('length'))) {
            return "Invalid length.";
        }
        if (!Utils.isNumber(this.get('innerWidth'))) {
            return "Invalid innerWidth.";
        }
        if (!Utils.isNumber(this.get('innerLength'))) {
            return "Invalid innerLength.";
        }

        return null;
    }),
    fromJson: function (json) {
        this.set('width', Utils.getEffectValueOrDefault(json, 0, "0"));
        this.set('length', Utils.getEffectValueOrDefault(json, 1, "0"));
        this.set('innerWidth', Utils.getEffectValueOrDefault(json, 2, "0"));
        this.set('innerLength', Utils.getEffectValueOrDefault(json, 3, "0"));
    },
    toJson: function () {
        return [
            Number(this.get('width')),
            Number(this.get('length')),
            Number(this.get('innerWidth')),
            Number(this.get('innerLength')),
        ];

    },
});

ConstantCuboidOriginSurface = OriginSurface.extend({
    componentName: 'cuboid-scalar-parameter',
    width: '1',
    length: '1',
    depth: '1',
    innerWidth: '0',
    innerLength: '0',
    innerDepth: '0',
    isValid: Ember.computed('width','length','depth','innerWidth','innerLength','innerDepth', function () {
        return Utils.isNumber(this.width) && Utils.isNumber(this.length) && Utils.isNumber(this.depth) &&
            Utils.isNumber(this.innerWidth) && Utils.isNumber(this.innerLength) && Utils.isNumber(this.innerDepth);
    }),
    invalidValueMessage: Ember.computed('width','length','depth','innerWidth','innerLength','innerDepth', function () {
        if (!Utils.isNumber(this.get('width'))) {
            return "Invalid width.";
        }
        if (!Utils.isNumber(this.get('length'))) {
            return "Invalid length.";
        }
        if (!Utils.isNumber(this.get('depth'))) {
            return "Invalid depth.";
        }
        if (!Utils.isNumber(this.get('innerWidth'))) {
            return "Invalid innerWidth.";
        }
        if (!Utils.isNumber(this.get('innerLength'))) {
            return "Invalid innerLength.";
        }
        if (!Utils.isNumber(this.get('innerDepth'))) {
            return "Invalid innerDepth.";
        }

        return null;
    }),
    fromJson: function (json) {
        this.set('width', Utils.getEffectValueOrDefault(json, 0, "0"));
        this.set('length', Utils.getEffectValueOrDefault(json, 1, "0"));
        this.set('depth', Utils.getEffectValueOrDefault(json, 2, "0"));
        this.set('innerWidth', Utils.getEffectValueOrDefault(json, 3, "0"));
        this.set('innerLength', Utils.getEffectValueOrDefault(json, 4, "0"));
        this.set('innerDepth', Utils.getEffectValueOrDefault(json, 5, "0"));
    },
    toJson: function () {
        return [
            Number(this.width),
            Number(this.length),
            Number(this.depth),
            Number(this.innerWidth),
            Number(this.innerLength),
            Number(this.innerDepth)
        ];
    },
});

ConstantSphereOriginSurface = OriginSurface.extend({
    componentName: 'sphere-scalar-parameter',
    radius: '1',
    innerRadius: '0',
    angle: '360',
    fromJson: function (json) {
        this.set('radius', Utils.getEffectValueOrDefault(json, 0, "0"));
        this.set('innerRadius', Utils.getEffectValueOrDefault(json, 1, "0"));
        this.set('angle', Utils.getEffectValueOrDefault(json, 2, "0"));
    },
    toJson: function () {
        return [
            Number(this.radius),
            Number(this.innerRadius),
            Number(this.angle),
        ];
    },
    isValid: Ember.computed('radius','innerRadius','angle', function () {
        return Utils.isNumber(this.radius) && Utils.isNumber(this.innerRadius) && Utils.isNumber(this.angle);
    }),
    invalidValueMessage: Ember.computed('radius','innerRadius','angle', function () {
        if (!Utils.isNumber(this.get('radius'))) {
            return "Invalid radius.";
        }
        if (!Utils.isNumber(this.get('innerRadius'))) {
            return "Invalid innerRadius.";
        }
        if (!Utils.isNumber(this.get('angle'))) {
            return "Invalid angle.";
        }

        return null;
    }),
});

ConstantCylinderOriginSurface = OriginSurface.extend({
    componentName: 'cylinder-scalar-parameter',
    height: '1',
    radius: '2',
    innerRadius: '0',
    angle: '360',
    fromJson: function (json) {
        this.set('height', Utils.getEffectValueOrDefault(json, 0, "0"));
        this.set('radius', Utils.getEffectValueOrDefault(json, 1, "0"));
        this.set('innerRadius', Utils.getEffectValueOrDefault(json, 2, "0"));
        this.set('angle', Utils.getEffectValueOrDefault(json, 3, "0"));
    },
    toJson: function () {
        return [
            Number(this.height),
            Number(this.radius),
            Number(this.innerRadius),
            Number(this.angle),
        ];
    },
    isValid: Ember.computed('height','radius','innerRadius','angle', function () {
        return Utils.isNumber(this.height) && Utils.isNumber(this.radius) && Utils.isNumber(this.innerRadius) && Utils.isNumber(this.angle);
    }),
    invalidValueMessage: Ember.computed('height','radius','innerRadius','angle', function () {
        if (!Utils.isNumber(this.get('height'))) {
            return "Invalid height.";
        }
        if (!Utils.isNumber(this.get('radius'))) {
            return "Invalid radius.";
        }
        if (!Utils.isNumber(this.get('innerRadius'))) {
            return "Invalid innerRadius.";
        }
        if (!Utils.isNumber(this.get('angle'))) {
            return "Invalid angle.";
        }

        return null;
    }),
});

ConstantConeOriginSurface = OriginSurface.extend({
    componentName: 'cone-scalar-parameter',
    radius: '1',
    innerRadius: '0',
    angle: '45',
    innerAngle: '0',
    fromJson: function (json) {
        this.set('radius', Utils.getEffectValueOrDefault(json, 0, "0"));
        this.set('innerRadius', Utils.getEffectValueOrDefault(json, 1, "0"));
        this.set('angle', Utils.getEffectValueOrDefault(json, 2, "0"));
        this.set('innerAngle', Utils.getEffectValueOrDefault(json, 3, "0"));
    },
    toJson: function () {
        return [
            Number(this.radius),
            Number(this.innerRadius),
            Number(this.angle),
            Number(this.innerAngle),
        ];
    },
    isValid: Ember.computed('radius','innerRadius','angle','innerAngle', function () {
        return Utils.isNumber(this.radius) && Utils.isNumber(this.innerRadius) && Utils.isNumber(this.angle) &&Utils.isNumber(this.innerAngle);
    }),
    invalidValueMessage: Ember.computed('radius','innerRadius','angle','innerAngle', function () {
        if (!Utils.isNumber(this.get('radius'))) {
            return "Invalid radius.";
        }
        if (!Utils.isNumber(this.get('innerRadius'))) {
            return "Invalid innerRadius.";
        }
        if (!Utils.isNumber(this.get('angle'))) {
            return "Invalid angle.";
        }
        if (!Utils.isNumber(this.get('innerAngle'))) {
            return "Invalid innerAngle.";
        }

        return null;
    }),
});

OriginSurfaceRegistry = {
    _options: [
        { surface: 'POINT', dimension: 'scalar', timeVarying: false, type: ConstantPointOriginSurface, },
        { surface: 'RECTANGLE', dimension: 'scalar', timeVarying: false, type: ConstantRectangleOriginSurface, },
        { surface: 'CUBOID', dimension: 'scalar', timeVarying: false, type: ConstantCuboidOriginSurface, },
        { surface: 'SPHERE', dimension: 'scalar', timeVarying: false, type: ConstantSphereOriginSurface, },
        { surface: 'CYLINDER', dimension: 'scalar', timeVarying: false, type: ConstantCylinderOriginSurface, },
        { surface: 'CONE', dimension: 'scalar', timeVarying: false, type: ConstantConeOriginSurface, },
    ],

    get: function (surface, dimension) {
        Utils.assert(typeof surface === 'string');
        Utils.assert(typeof dimension === 'string');

        for (var i = 0; i < OriginSurfaceRegistry._options.length; i++) {
            var option = OriginSurfaceRegistry._options[i];
            if (option.surface === surface && option.dimension === dimension) {
                var args = option.args || {};
                return option.type.create(args);
            }
        }

        Utils.assert(false, "parameter surface " + surface + " with dimension " + dimension + " not found valid");
    },

    getNames: function (dimension, timeVarying) {
        var options = [];
        for (var i = 0; i < OriginSurfaceRegistry._options.length; i++) {
            var option = OriginSurfaceRegistry._options[i];
            var timeVaryingMatches = !(option.timeVarying && !timeVarying);
            if (option.dimension === dimension && timeVaryingMatches) {
                options.push(option.surface);
            }
        }

        return options;
    },
};

OriginProperty = EffectProperty.extend({
    componentName: 'origin-parameter-property',
    timeVarying: false,
    dimension: 'scalar',
    parameter: null,
    surfaceOptionNames: null,
    surface: null,

    isMissing: false,
    isValid: Ember.computed('parameter.isValid', function () {
        return this.parameter === null || this.parameter.get('isValid');
    }),
    _onInit: function(){
        this.set('surfaceOptionNames', OriginSurfaceRegistry.getNames(this.dimension, this.timeVarying));
    }.on('init'),
    toJson: function () {
        return {
            surface: this.surface,
            values: this.get('parameter').toJson(),
        };
    },
    fromJson: function (json) {
        Utils.assert(Utils.isUndefinedOrTypeOf("object", json));
        var isMissing = json === undefined;
        this.set('isMissing', isMissing);
        if (isMissing) {
            this.set('surface', 'POINT');
            if (!this.get('optional')) {
                this.set('isMissing', false);
            }
            return;
        }
        var surface = json['surface'];
        this.set('surface', surface);
        this.parameter.fromJson(json['values']);
    },
    _surfaceObserver: Ember.observer('surface', function (sender, key, value, rev) {
        this.set('parameter', OriginSurfaceRegistry.get(this.surface, this.dimension));
    }),
});

