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

CurveScalarParameterKind = Ember.Object.extend({
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
        this.set('parameter', CurveScalarParameterKind.create({}));
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

