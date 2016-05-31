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
            return;
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
            return;
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

OriginProperty = EffectProperty.extend({
    componentName: 'origin-property',
    surface: null, // string or null
    value1: '', // string
    value2: '', // string
    isMissing: null,
    isValid: Ember.computed('value1', 'value2', function () {
        return Utils.isNumber(this.get('value1')) && Utils.isNumber(this.get('value2'));
    }),
    invalidValue1Message: Ember.computed('value1', function () {
        if (Utils.isNumber(this.get('value1'))) {
            return null;
        }

        return "Invalid number";
    }),
    invalidValue2Message: Ember.computed('value2', function () {
        if (Utils.isNumber(this.get('value2'))) {
            return null;
        }

        return "Invalid number";
    }),
    toJson: function () {
        return {
            surface: this.get('surface'),
            values: [
                Number(this.get('value1')),
                Number(this.get('value2'), 10),
            ],
        };
    },
    fromJson: function (json) {
        Utils.assert(Utils.isUndefinedOrTypeOf("object", json));
        if (json === undefined) {
            return;
        }
        this.set('surface', json['surface']);
        this.set('value1', json['values'][0].toString());
        this.set('value2', json['values'][1].toString());
    },
});

ParameterKind = Ember.Object.extend({
    kind: null,
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

DummyParameterKind = ParameterKind.extend({
    kind: 'DUMMY',
    rawJson: null,
    fromJson: function (json) {
        this.set('rawJson', json);
    },
    toJson: function (json) {
        return this.get('rawJson');
    },
    isValid: function () {
        return true;
    },
});

ParameterKindRegistry = {
    _options: [
        { kind: 'CONSTANT', dimension: 'rgba', timeVarying: false, type: DummyParameterKind, },
        { kind: 'CONSTANT', dimension: 'scalar', timeVarying: false, type: DummyParameterKind, },
        { kind: 'CURVE', dimension: 'scalar', timeVarying: true, type: DummyParameterKind, },
        { kind: 'RANDOM_BETWEEN_CURVES', dimension: 'scalar', timeVarying: true, type: DummyParameterKind, },
        { kind: 'RANDOM_BETWEEN', dimension: 'scalar', timeVarying: false, type: DummyParameterKind, },
    ],

    get: function (kind, dimension) {
        for (var i = 0; i < ParameterKindRegistry._options.length; i++) {
            var option = ParameterKindRegistry._options[i];
            if (option.kind === kind && option.dimension === dimension) {
                return option.type.create({});
            }
        }

        Utils.assert(false);
    },
};

ParameterProperty = EffectProperty.extend({
    componentName: 'parameter-property',
    timeVarying: false,
    dimension: 'scalar',
    parameter: null,

    isMissing: false,
    isValid: Ember.computed('value1', 'value2', function () {
        return this.parameter === null || this.parameter.isValid();
    }),
    toJson: function () {
        return {
            kind: this.get('parameter').kind,
            value: this.get('parameter').toJson(),
        };
    },
    fromJson: function (json) {
        Utils.assert(Utils.isUndefinedOrTypeOf("object", json));
        if (json === undefined) {
            return;
        }
        var kind = json['kind'];
        this.set('parameter', ParameterKindRegistry.get(kind, this.dimension));
        this.parameter.fromJson(json['values']);
    },
});

