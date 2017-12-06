StringProperty = EffectProperty.extend({
    componentName: 'string-property',
    value: '', // string
    isMissing: Ember.computed('value', function () {
        return this.get('value') === undefined;
    }),
    isValid: function () {
        return true;
    }.property(),
    toJson: function () {
        return this.get('value');
    },
    fromJson: function (json) {
        Utils.assert(Utils.isUndefinedOrTypeOf("string", json));
        if (json === undefined) {
            json = '0';
        }
        this.set('value', json);
    }
});

MaterialProperty = EffectProperty.extend({
    componentName: 'material-property',
    value: '', // string
    isMissing: Ember.computed('value', function () {
        return this.get('value') === undefined;
    }),
    isValid: function () {
        return true;
    }.property(),
    toJson: function () {
        return this.get('value');
    },
    fromJson: function (json) {
        Utils.assert(Utils.isUndefinedOrTypeOf("string", json));
        if (json === undefined) {
            json = 'materials/cubemitter.material.json';
        }
        this.set('value', json);
    }
});

IntProperty = EffectProperty.extend({
    componentName: 'int-property',
    value: '', // string, undefined, null
    isMissing: Ember.computed('value', function () {
        return this.get('value') === undefined;
    }),
    isValid: Ember.computed('value', function () {
        return Utils.isNumber(this.get('value'));
    }),
    toJson: function () {
        return Number(this.get('value'));
    },
    fromJson: function (json) {
        Utils.assert(Utils.isUndefinedOrTypeOf("number", json));
        if (json === undefined) {
            json = '0';
        }
        this.set('value', json.toString());
    },
    invalidValueMessage: Ember.computed('value', function () {
        if (Utils.isNumber(this.get('value'))) {
            return null;
        }

        return "Invalid number";
    }),
});

BooleanProperty = EffectProperty.extend({
    componentName: 'boolean-property',
    value: true,
    isMissing: Ember.computed('value', function () {
        return this.get('value') === undefined;
    }),
    isValid: Ember.computed('value', function () {
        return true;
    }),
    toJson: function () {
        return this.get('value');
    },
    fromJson: function (json) {
        Utils.assert(Utils.isUndefinedOrTypeOf("boolean", json));
        if (json === undefined) {
            json = true;
        }
        this.set('value', json);
    },
    invalidValueMessage: Ember.computed('value', function () {
        return null;
    }),
});

ParameterKind = Ember.Object.extend({
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

ConstantScalarParameterKind = ParameterKind.extend({
    componentName: 'constant-scalar-parameter',
    value: '0',
    fromJson: function (json) {
        if (json === undefined) {
            this.set('value', '0');
            // This should detect if 'rate' is the name of the parameter we are inspecting, and if so set the value to 50.
            if (this.get('name') === 'rate') {
                this.set('value', '50');
            }
            return;
        }
        this.set('value', Utils.getEffectValueOrDefault(json, 0, "0"));
    },
    toJson: function () {
        return [Number(this.value)];
    },
    isValid: Ember.computed('value', function () {
        return Utils.isNumber(this.value);
    }),
    invalidValueMessage: Ember.computed('value', function () {
        if (Utils.isNumber(this.get('value'))) {
            return null;
        }

        return "Invalid number";
    }),
});

ConstantRgbaParameterKind = ParameterKind.extend({
    componentName: 'constant-rgba-parameter',
    id: '',
    hasA: true,
    rgba: null,
    colorPicker: null,
    _onInit: function () {
        var self = this;
        self.set('rgba', Rgba.create({ hasA: self.get('hasA') }));
        self.set('id', 'color' + ParameterKindRegistry.getID());
        Ember.run.schedule('afterRender', self, function () {
            self._setupColorPicker();
        });
    }.on('init'),
    _setupColorPicker: function () {
        var self = this;
        var picker = $('#' + self.get('id') + ' .color-picker');
        self.set('colorPicker', picker);
        picker.spectrum({
            color: "#ff0000",
            showAlpha: self.get('hasA'),
            showInput: true,
            showInitial: true,
            preferredFormat: "rgb",
            change: function (color) {
                self.updateColor(color);
            }
        });
    },
    updateColor: function (color) {
        var floatVals = Utils.convertRgbaToFloat(color._r, color._g, color._b, color._a);
        this.get('rgba').setRgba(floatVals.r, floatVals.g, floatVals.b, floatVals.a);
    },
    fromJson: function (json) {
        var self = this;
        var rgba = this.get('rgba');
        Ember.run.scheduleOnce('afterRender', this, function () {
            rgba.setPickerColor(self.get('colorPicker'));
        });
        rgba.fromJson(json);
    },
    toJson: function () {
        return this.rgba.toJson();
    },
    isValid: Ember.computed('rgba.isValid', function () {
        return this.get('rgba.isValid');
    }),
});

RandomBetweenRgbaParameterKind = ParameterKind.extend({
    componentName: 'random-between-rgba-parameter',
    hasA: true,
    id1: '',
    id2: '',
    rgba1: null,
    rgba2: null,
    colorPicker1: null,
    colorPicker2: null,
    _onInit: function () {
        var self = this;
        self.set('rgba1', Rgba.create({ hasA: this.get('hasA') }));
        self.set('rgba2', Rgba.create({ hasA: this.get('hasA') }));

        self.set('id1', 'color' + ParameterKindRegistry.getID());
        self.set('id2', 'color' + ParameterKindRegistry.getID());

        Ember.run.scheduleOnce('afterRender', self, function () {
            var name1 = '#' + self.get('id1') + ' .color-picker';
            var picker1 = $(name1);
            self.set('colorPicker1', picker1);
            picker1.spectrum({
                color: "#ff0000",
                showAlpha: self.get('hasA'),
                showInput: true,
                showInitial: true,
                preferredFormat: "rgb",
                change: function (color) {
                    self.updateColor(color, 'rgba1');
                }
            });
            var name2 = '#' + self.get('id2') + ' .color-picker';
            var picker2 = $(name2);
            self.set('colorPicker2', picker2);
            picker2.spectrum({
                color: "#ff0000",
                showAlpha: self.get('hasA'),
                showInput: true,
                showInitial: true,
                preferredFormat: "rgb",
                change: function (color) {
                    self.updateColor(color, 'rgba2');
                }
            });
        });
    }.on('init'),
    updateColor: function (color, id) {
        var floatVals = Utils.convertRgbaToFloat(color._r, color._g, color._b, color._a);
        this.get(id).setRgba(floatVals.r, floatVals.g, floatVals.b, floatVals.a);
    },
    fromJson: function (json) {
        var self = this;
        var rgba1 = this.get('rgba1');
        var rgba2 = this.get('rgba2');

        Ember.run.scheduleOnce('afterRender', this, function () {
            rgba1.setPickerColor(self.get('colorPicker1'));
        });
        Ember.run.scheduleOnce('afterRender', this, function () {
            rgba2.setPickerColor(self.get('colorPicker2'));
        });

        rgba1.fromJson(json[0]);
        rgba2.fromJson(json[1]);
    },
    toJson: function () {
        return [this.rgba1.toJson(), this.rgba2.toJson()];
    },
    isValid: Ember.computed('rgba1.isValid', 'rgba2.isValid', function () {
        return this.rgba1.get('isValid') && this.rgba2.get('isValid');
    }),
});

CurveScalarParameterKind = ParameterKind.extend({
    componentName: 'curve-scalar-parameter',
    curve: null,
    _onInit: function () {
        this.set('curve', Curve.create({}));
    }.on('init'),
    fromJson: function (json) {
        var curve = Curve.create({});
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

CurveRgbParameterKind = ParameterKind.extend({
    componentName: 'curve-rgb-parameter',
    curveRGB: null,
    _onInit: function () {
        this.set('curveRGB', CurveRGB.create({}));
    }.on('init'),
    fromJson: function (json) {
        var curveRGB = CurveRGB.create({});
        curveRGB.fromJson(json);
        this.set('curveRGB', curveRGB);
    },
    toJson: function () {
        return this.curveRGB.toJson();
    },
    isValid: Ember.computed('curveRGB.isValid', function () {
        return this.get('curveRGB.isValid');
    }),
});

RandomBetweenScalarParameterKind = ParameterKind.extend({
    componentName: 'random-between-scalar-parameter',
    minValue: '0',
    maxValue: '0.1',
    fromJson: function (json) {
        this.set('minValue', Utils.getEffectValueOrDefault(json, 0, '0'));
        this.set('maxValue', Utils.getEffectValueOrDefault(json, 1, '0'));
    },
    toJson: function () {
        return [Number(this.minValue), Number(this.maxValue)];
    },
    isValid: Ember.computed('minValue', 'maxValue', function () {
        return Utils.isNumber(this.minValue) && Utils.isNumber(this.maxValue);
    }),
    invalidMinValueMessage: Ember.computed('minValue', function () {
        if (Utils.isNumber(this.get('minValue'))) {
            return null;
        }

        return "Invalid number";
    }),
    invalidMaxValueMessage: Ember.computed('maxValue', 'minValue', function () {
        if (!Utils.isNumber(this.get('maxValue'))) {
            return "Invalid number";
        }

        if (Utils.isNumber(this.get('minValue'))) {
            var min = Number(this.minValue);
            var max = Number(this.maxValue);
            if (min >= max) {
                return "Must be greater than min";
            }
        }

        return null;
    }),
});

Point = Ember.Object.extend({
    time: '0',
    value: '0',

    isValid: Ember.computed('time', 'value', function () {
        return Utils.isNumber(this.time) && Utils.isNumber(this.value);
    }),
    invalidMessage: Ember.computed('time', 'value', function () {
        if (!Utils.isNumber(this.time)) {
            return "Invalid time.";
        }
        if (!Utils.isNumber(this.value)) {
            return "Invalid value.";
        }

        return null;
    }),
    fromJson: function (json) {
        this.set('time', Utils.getEffectValueOrDefault(json, 0, '0'));
        this.set('value', Utils.getEffectValueOrDefault(json, 1, '0'));
    },
    toJson: function () {
        return [Number(this.time), Number(this.value)];
    },
});

Curve = Ember.Object.extend({
    points: null,
    _onInit: function () {
        this.set('points', Ember.A());
    }.on('init'),
    fromJson: function (json) {
        var points = Ember.A();
        for (var i = 0; i < json.length; i++) {
            var point = Point.create({});
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
    errorMap: Ember.computed('points.@each.time', 'points.@each.value', function () {
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
        if (points.length < 2) {
            addError(-1, "Need at least two points in curve.");
        }
        for (var i = 0; i < points.length; i++) {
            var pointMessage = points[i].get('invalidMessage');
            if (pointMessage) {
                addError(i, pointMessage);
            }
        }

        if (points.length > 0) {
            if (Number(points[0].time) !== 0) {
                addError(0, "First point must have time=0.");
            }
        }
        for (var i = 0; i < points.length - 1; i++) {
            if (Number(points[i].time) >= Number(points[i + 1].time)) {
                addError(i + 1, "Time must be greater than last point's time.");
            }
        }

        return ret;
    }),
    isValid: Ember.computed('errorMap', function () {
        return Object.keys(this.get('errorMap')).length === 0;
    }),
});

PointRgb = Ember.Object.extend({
    hasA: false,
    id: '',
    time: '0',
    rValue: '0',
    gValue: '0',
    bValue: '0',
    aValue: '1',
    colorPicker: null,
    _onInit: function () {
        var self = this;
        self.set('id', 'color' + ParameterKindRegistry.getID());
        Ember.run.scheduleOnce('afterRender', self, function () {
            var picker = $('#' + self.get('id') + ' .color-picker');
            self.set('colorPicker', picker);
            picker.spectrum({
                color: "#ff0000",
                showAlpha: self.get('hasA'),
                showInput: true,
                showInitial: true,
                preferredFormat: "rgb",
                change: function (color) {
                    self.updateColor(color);
                }
            });
        });
    }.on('init'),
    updateColor: function (color) {
        var floatVals = Utils.convertRgbaToFloat(color._r, color._g, color._b, color._a);
        this.setRgba(floatVals.r, floatVals.g, floatVals.b, floatVals.a);
    },
    fromJson: function (json) {
        this.set('time', Utils.getEffectValueOrDefault(json, 0, '0'));
        var self = this;
        var r = Utils.getEffectValueOrDefault(json, 1, '0');
        var g = Utils.getEffectValueOrDefault(json, 2, '0');
        var b = Utils.getEffectValueOrDefault(json, 3, '0');
        var a = Utils.getEffectValueOrDefault(json, 3, '0');
        var a;
        if (!this.get('hasA')) {
            a = '1';
        } else {
            a = Utils.getEffectValueOrDefault(json, 3, '0');
        }
        self.setRgba(r, g, b, a);
        Ember.run.scheduleOnce('afterRender', this, function () {
            self.get('colorPicker').spectrum("set", Utils.convertFloatToRgba(r,g,b,a));
        });
    },
    toJson: function () {
        var json = [Number(this.time), Number(this.rValue), Number(this.gValue), Number(this.bValue)];
        return json;
    },
    isValid: Ember.computed('time', 'rValue', 'gValue', 'bValue', 'aValue', function () {
        return Utils.isNumber(this.time) && Utils.isNumber(this.rValue) && Utils.isNumber(this.gValue) && Utils.isNumber(this.bValue) && Utils.isNumber(this.aValue);
    }),
    setRgba: function (r, g, b, a) {
        this.set('rValue', r);
        this.set('gValue', g);
        this.set('bValue', b);
        this.set('aValue', a);
    },
    setPickerColor: function(picker) {
        this.set('colorPicker', picker);
    },
    invalidMessage: Ember.computed('time', 'rValue', 'gValue', 'bValue', 'aValue', function () {
        if (!Utils.isNumber(this.get('time'))) {
            return "Invalid time.";
        }
        if (!Utils.isNumber(this.get('rValue'))) {
            return "Invalid rValue.";
        }
        if (!Utils.isNumber(this.get('gValue'))) {
            return "Invalid gValue.";
        }
        if (!Utils.isNumber(this.get('bValue'))) {
            return "Invalid bValue.";
        }
        if (!Utils.isNumber(this.get('aValue'))) {
            return "Invalid aValue.";
        }

        return null;
    }),
});

CurveRGB = Ember.Object.extend({
    pointsRGB: null,
    _onInit: function () {
        this.set('pointsRGB', Ember.A());
    }.on('init'),
    fromJson: function (json) {
        var pointsRGB = Ember.A();
        for (var i = 0; i < json.length; i++) {
            var point = PointRgb.create({});
            point.fromJson(json[i]);
            pointsRGB.push(point);
        }
        this.set('pointsRGB', pointsRGB);
    },
    toJson: function () {
        var ret = [];
        for (var i = 0; i < this.pointsRGB.length; i++) {
            ret.push(this.pointsRGB[i].toJson());
        }
        return ret;
    },
    errorMap: Ember.computed('pointsRGB.@each.time', 'pointsRGB.@each.rValue', 'pointsRGB.@each.gValue', 'pointsRGB.@each.bValue', function () {
        // Returns index -> error message, -1 means overall
        var ret = {};
        function addError(index, message) {
            if (index in ret) {
                ret[index] += " " + message;
            } else {
                ret[index] = message;
            }
        }

        var pointsRGB = this.get('pointsRGB');
        if (pointsRGB === null) {
            return {};
        }
        if (pointsRGB.length < 2) {
            addError(-1, "Need at least two points in curve.");
        }
        for (var i = 0; i < pointsRGB.length; i++) {
            var pointMessage = pointsRGB[i].get('invalidMessage');
            if (pointMessage) {
                addError(i, pointMessage);
            }
        }

        if (pointsRGB.length > 0) {
            if (Number(pointsRGB[0].time) !== 0) {
                addError(0, "First point must have time=0.");
            }
        }
        for (var i = 0; i < pointsRGB.length - 1; i++) {
            if (Number(pointsRGB[i].time) >= Number(pointsRGB[i + 1].time)) {
                addError(i + 1, "Time must be greater than last point's time.");
            }
        }

        return ret;
    }),
    isValid: Ember.computed('errorMap', function () {
        return Object.keys(this.get('errorMap')).length === 0;
    }),
});

Rgba = Ember.Object.extend({
    hasA: true,
    rValue: '0',
    gValue: '0',
    bValue: '0',
    aValue: '1',
    colorPicker: null,
    fromJson: function (json) {
        var self = this;
        var r = Utils.getEffectValueOrDefault(json, 0, '0');
        var g = Utils.getEffectValueOrDefault(json, 1, '0');
        var b = Utils.getEffectValueOrDefault(json, 2, '0');
        var a;
        if (!this.get('hasA')) {
            a = '1';
        } else {
            a = Utils.getEffectValueOrDefault(json, 3, '0');
        }
        self.setRgba(r, g, b, a);
        Ember.run.scheduleOnce('afterRender', this, function () {
            self.get('colorPicker').spectrum("set", Utils.convertFloatToRgba(r,g,b,a));
        });
    },
    toJson: function () {
        var json = [Number(this.rValue), Number(this.gValue), Number(this.bValue)];
        if (this.get('hasA')) {
            json.push(Number(this.aValue));
        }
        return json;
    },
    isValid: Ember.computed('rValue', 'gValue', 'bValue', 'aValue', function() {
        return Utils.isNumber(this.rValue) && Utils.isNumber(this.gValue) && Utils.isNumber(this.bValue) && Utils.isNumber(this.aValue);
    }),
    setRgba: function (r, g, b, a) {
        this.set('rValue', r);
        this.set('gValue', g);
        this.set('bValue', b);
        this.set('aValue', a);
    },
    setPickerColor: function(picker) {
        this.set('colorPicker', picker);
    },
    invalidRValueMessage: Ember.computed('rValue', function () {
        if (Utils.isNumber(this.get('rValue'))) {
            return null;
        }

        return "Invalid number";
    }),
    invalidGValueMessage: Ember.computed('gValue', function () {
        if (Utils.isNumber(this.get('gValue'))) {
            return null;
        }

        return "Invalid number";
    }),
    invalidBValueMessage: Ember.computed('bValue', function () {
        if (Utils.isNumber(this.get('bValue'))) {
            return null;
        }

        return "Invalid number";
    }),
    invalidAValueMessage: Ember.computed('aValue', function () {
        if (Utils.isNumber(this.get('aValue'))) {
            return null;
        }

        return "Invalid number";
    }),
});

RandomBetweenCurvesScalarParameterKind = ParameterKind.extend({
    componentName: 'random-between-curves-scalar-parameter',
    curve1: null,
    curve2: null,
    _onInit: function () {
        this.set('curve1', Curve.create({}));
        this.set('curve2', Curve.create({}));
    }.on('init'),
    fromJson: function (json) {
        var curve1 = Curve.create({});
        curve1.fromJson(json[0]);
        this.set('curve1', curve1);

        var curve2 = Curve.create({});
        curve2.fromJson(json[1]);
        this.set('curve2', curve2);
    },
    toJson: function () {
        return [this.curve1.toJson(), this.curve2.toJson()];
    },
    isValid: Ember.computed('curve1.isValid', 'curve2.isValid', function () {
        return this.curve1.get('isValid') && this.curve2.get('isValid');
    }),
});

RandomBetweenCurvesRgbParameterKind = ParameterKind.extend({
    componentName: 'random-between-curves-rgb-parameter',
    curveRGB1: null,
    curveRGB2: null,
    _onInit: function () {
        this.set('curveRGB1', CurveRGB.create({}));
        this.set('curveRGB2', CurveRGB.create({}));
    }.on('init'),
    fromJson: function (json) {
        var curveRGB1 = CurveRGB.create({});
        curveRGB1.fromJson(json[0]);
        this.set('curveRGB1', curveRGB1);

        var curveRGB2 = CurveRGB.create({});
        curveRGB2.fromJson(json[1]);
        this.set('curveRGB2', curveRGB2);
    },
    toJson: function () {
        return [this.curveRGB1.toJson(), this.curveRGB2.toJson()];
    },
    isValid: Ember.computed('curveRGB1.isValid', 'curveRGB2.isValid', function () {
        return this.curveRGB1.get('isValid') && this.curveRGB2.get('isValid');
    }),
});

PointBurst = Ember.Object.extend({
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

CurveBurst = Ember.Object.extend({
    points: null,
    isMissing: null,
    _onInit: function () {
        this.set('points', Ember.A());
    }.on('init'),
    fromJson: function (json) {
        var points = Ember.A();
        for (var i = 0; i < json.length; i++) {
            var point = PointBurst.create({});
            point.fromJson(json[i]);
            points.push(point);
        }
        var isMissing = json === undefined;
        this.set('isMissing', isMissing);
        console.log(json)
        if (isMissing) {
            this.set('points', ['0','0']);
            return;
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

BurstScalarParameterKind = Ember.Object.extend({
    componentName: 'burst-scalar-parameter',
    curve: null,
    _onInit: function () {
        this.set('curve', CurveBurst.create({}));
    }.on('init'),
    fromJson: function (json) {
        var curve = CurveBurst.create({});
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

ParameterKindRegistry = {
    _idCount: 0,
    getID: function() {
        ParameterKindRegistry._idCount++;
        return ParameterKindRegistry._idCount;
    },

    _options: [
        { kind: 'CONSTANT', dimension: 'rgba', timeVarying: false, bursts: false, type: ConstantRgbaParameterKind, },
        { kind: 'CONSTANT', dimension: 'rgb', timeVarying: false, bursts: false, type: ConstantRgbaParameterKind, args: { hasA: false, }, },
        { kind: 'CONSTANT', dimension: 'scalar', timeVarying: false, bursts: false, type: ConstantScalarParameterKind, },
        { kind: 'CURVE', dimension: 'scalar', timeVarying: true, bursts: false, type: CurveScalarParameterKind, },
        { kind: 'CURVE', dimension: 'rgb', timeVarying: true, bursts: false, type: CurveRgbParameterKind, },
        { kind: 'RANDOM_BETWEEN_CURVES', dimension: 'scalar', timeVarying: true, bursts: false, type: RandomBetweenCurvesScalarParameterKind, },
        { kind: 'RANDOM_BETWEEN_CURVES', dimension: 'rgb', timeVarying: true, bursts: false, type: RandomBetweenCurvesRgbParameterKind, },
        { kind: 'RANDOM_BETWEEN', dimension: 'scalar', timeVarying: false, bursts: false, type: RandomBetweenScalarParameterKind, },
        { kind: 'RANDOM_BETWEEN', dimension: 'rgba', timeVarying: false, bursts: false, type: RandomBetweenRgbaParameterKind, },
        { kind: 'RANDOM_BETWEEN', dimension: 'rgb', timeVarying: false, bursts: false, type: RandomBetweenRgbaParameterKind, args: { hasA: false, }, },
        { kind: 'BURST', dimension: 'scalar', timeVarying: true, bursts: true, type: BurstScalarParameterKind, args: { hasA: false, }, },
    ],

    get: function (kind, dimension) {
        Utils.assert(typeof kind === 'string');
        Utils.assert(typeof dimension === 'string');

        for (var i = 0; i < ParameterKindRegistry._options.length; i++) {
            var option = ParameterKindRegistry._options[i];
            if (option.kind === kind && option.dimension === dimension) {
                var args = option.args || {};
                return option.type.create(args);
            }
        }

        Utils.assert(false, "parameter kind " + kind + " with dimension " + dimension + " not found valid");
    },

    getNames: function (dimension, timeVarying, bursts) {
        var options = [];
        for (var i = 0; i < ParameterKindRegistry._options.length; i++) {
            var option = ParameterKindRegistry._options[i];
            var timeVaryingMatches = !(option.timeVarying && !timeVarying);
            var burstsMatches = !(option.bursts && !bursts);
            if (option.dimension === dimension && timeVaryingMatches && burstsMatches) {
                options.push(option.kind);
            }
        }

        return options;
    },
};

ParameterProperty = EffectProperty.extend({
    componentName: 'parameter-property',
    timeVarying: false,
    dimension: 'scalar',
    parameter: null,
    kindOptionNames: null,
    kind: null,
    bursts: false,
    isMissing: false,
    isValid: Ember.computed('parameter.isValid', function () {
        return this.parameter === null || this.parameter.get('isValid');
    }),
    _onInit: function(){
        this.set('kindOptionNames', ParameterKindRegistry.getNames(this.dimension, this.timeVarying, this.bursts));
    }.on('init'),
    toJson: function () {
        return {
            kind: this.kind,
            values: this.get('parameter').toJson(),
        };
    },
    fromJson: function (json) {
        Utils.assert(Utils.isUndefinedOrTypeOf("object", json));
        var isMissing = json === undefined;
        this.set('isMissing', isMissing);
        if (isMissing) {
            this.set('kind', 'CONSTANT');
            this.set('parameter', (this.kind, this.dimension));
            if (!this.get('optional')) {
                this.set('isMissing', false);
            }
            return;
        }
        var kind = json['kind'];
        this.set('kind', kind);
        this.parameter.fromJson(json['values']);
    },
    _kindObserver: Ember.observer('kind', function (sender, key, value, rev) {
        this.set('parameter', ParameterKindRegistry.get(this.kind, this.dimension));
    }),
    _missingObserver: Ember.observer('isMissing', function (sender, key, value, rev) {
        if (!this.isMissing) {
            this.set('parameter', ParameterKindRegistry.get(this.kind, this.dimension));
        }
    }),
});
