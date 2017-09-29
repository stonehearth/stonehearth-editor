EmissionKind = Ember.Object.extend({
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

Point_Burst = Ember.Object.extend({
    time: '0',
    burst: '0',

    isValid: Ember.computed('time', 'burst', function () {
        return Utils.isNumber(this.time) && Utils.isNumber(this.burst);
    }),
    invalidMessage: Ember.computed('time', 'burst', function () {
        if (!Utils.isNumber(this.time)) {
            return "Invalid time.";
        }
        if (!Utils.isNumber(this.burst)) {
            return "Invalid burst.";
        }

        return null;
    }),
    fromJson: function (json) {
        this.set('time', Utils.getEffectValueOrDefault(json, 0, '0'));
        this.set('burst', Utils.getEffectValueOrDefault(json, 1, '0'));
    },
    toJson: function () {
        return [Number(this.time), Number(this.burst)];
    },
});

Curve_Burst = Ember.Object.extend({
    points: null,
    _onInit: function () {
        this.set('points', Ember.A());
    }.on('init'),
    fromJson: function (json) {
        var points = Ember.A();
        for (var i = 0; i < json.length; i++) {
            var point = Point_Burst.create({});
            point.fromJson(json[i]);
            points.push(point);
        }
        this.set('points', points);
    },
    toJson: function () {
        var ret = [];
        for (var i = 0; i < this.points.length; i++) {
            ret.push(this.points[i].toJson());
        }
        return ret;
    },
    errorMap: Ember.computed('points.@each.time', 'points.@each.burst', function () {
        // Returns index -> error message, -1 means overall
        var ret = {};
        function addError(index, message) {
            if (index in ret) {
                ret[index] += " " + message;
            } else {
                ret[index] = message;
            }
        }

        var points = this.get('points');
        if (points === null) {
            return {};
        }
        for (var i = 0; i < points.length; i++) {
            var pointMessage = points[i].get('invalidMessage');
            if (pointMessage) {
                addError(i, pointMessage);
            }
        }

        return ret;
    }),
    isValid: Ember.computed('errorMap', function () {
        return Object.keys(this.get('errorMap')).length === 0;
    }),
});

BurstScalarEmissionKind = Ember.Object.extend({
    componentName: 'burst-scalar-parameter',
    curve: null,
    _onInit: function () {
        this.set('curve', Curve_Burst.create({}));
    }.on('init'),
    fromJson: function (json) {
        var curve = Curve_Burst.create({});
        curve.fromJson(json);
        this.set('curve', curve);
    },
    toJson: function () {
        return this.curve.toJson();
    },
    isValid: Ember.computed('curve.isValid', function () {
        return this.get('curve.isValid');
    }),
});

BurstProperty = EffectProperty.extend({
    componentName: 'burst-parameter-property',
    timeVarying: true,
    dimension: 'scalar',
    parameter: null,

    isMissing: false,
    isValid: Ember.computed('parameter.isValid', function () {
        return this.parameter === null || this.parameter.get('isValid');
    }),
    _onInit: function(){
        this.set('parameter', BurstScalarEmissionKind.create({}));
    }.on('init'),
    toJson: function () {
        return this.get('parameter').toJson();
    },
    fromJson: function (json) {
        Utils.assert(Utils.isUndefinedOrTypeOf("object", json));
        if(!json) {
            json = [[0,0]];
        }
        this.parameter.fromJson(json);
    }
});

EmissionKindRegistry = {
    _options: [
        { kind: 'RATE', dimension: 'scalar', timeVarying: false, type: RateScalarEmissionKind, },
        { kind: 'BURSTS', dimension: 'scalar', timeVarying: true, type: BurstScalarEmissionKind, },
    ],

    get: function (kind, dimension) {
        Utils.assert(typeof kind === 'string');
        Utils.assert(typeof dimension === 'string');

        for (var i = 0; i < EmissionKindRegistry._options.length; i++) {
            var option = EmissionKindRegistry._options[i];
            if (option.kind === kind && option.dimension === dimension) {
                var args = option.args || {};
                return option.type.create(args);
            }
        }

        Utils.assert(false, "parameter kind " + kind + " with dimension " + dimension + " not found valid");
    },

    getNames: function (dimension, timeVarying) {
        var options = [];
        for (var i = 0; i < EmissionKindRegistry._options.length; i++) {
            var option = EmissionKindRegistry._options[i];
            var timeVaryingMatches = !(option.timeVarying && !timeVarying);
            if (option.dimension === dimension && timeVaryingMatches) {
                options.push(option.kind);
            }
        }

        return options;
    },
};

EmissionProperty = EffectProperty.extend({
    componentName: 'emission-property',
    timeVarying: false,
    dimension: 'scalar',
    parameter: null,
    kindOptionNames: null,
    kind: null,

    isMissing: false,
    isValid: Ember.computed('parameter.isValid', function () {
        return this.parameter === null || this.parameter.get('isValid');
    }),
    _onInit: function(){
        this.set('kindOptionNames', EmissionKindRegistry.getNames(this.dimension, this.timeVarying));
    }.on('init'),
    toJson: function () {
        return {
            kind: this,
            values: this.get('parameter').toJson(),
        };
    },
    fromJson: function (json) {
        Utils.assert(Utils.isUndefinedOrTypeOf("object", json));
        var isMissing = json === undefined;
        this.set('isMissing', isMissing);
        if (isMissing) {
            this.set('kind', 'RATE');
            return;
        }
        var kind = json['kind'];
        this.set('kind', kind);
        this.parameter.fromJson(json['values']);
    },
    _kindObserver: Ember.observer('kind', function (sender, key, value, rev) {
        this.set('parameter', EmissionKindRegistry.get(this.kind, this.dimension));
    }),
});

