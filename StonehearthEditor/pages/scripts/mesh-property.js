MeshFileProperty = EffectProperty.extend({
    componentName: 'meshfile-property',
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
            json = '';
        }
        this.set('value', json);
    }
});

MeshMatrixProperty = EffectProperty.extend({
    componentName: 'meshmatrix-property',
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
            json = '';
        }
        this.set('value', json);
    }
});

MeshOffsetProperty = EffectProperty.extend({
    componentName: 'meshoffset-parameter-property',
    offset: { "x": 0, "y": 0, "z": 0 }, // dictionary
    isMissing: false,
    isValid: Ember.computed('parameter.isValid', function () {
        return true
    }),
    toJson: function () {
        return {
            "x": Number(this.offset.x),
            "y": Number(this.offset.y),
            "z": Number(this.offset.z)
        };
    },
    fromJson: function (json) {
        // json == { "x": 1, "y": 1, "z": 1 }
        // Utils.assert(Utils.isUndefinedOrTypeOf("object", json));
        var isMissing = json === undefined;
        // this.set('isMissing', isMissing);
        if (isMissing) {
            this.set('offset', { "x": 0, "y": 0, "z": 0 });
            return;
        }
        this.set('offset', {"x":json['x'], "y":json['y'], "z":json['z']});
    }
});
