ComplexProperty = EffectProperty.extend({
    children: null, // list of EffectProperty
    optional: null, // bool
    extra: null, // dictionary of extra fields
    _isMissing: null, // bool
    isMissing: Ember.computed('_isMissing', function () {
        return this.get('_isMissing');
    }),
    isValid: Ember.computed('children', function () {
        var children = this.get('children');
        for (var i = 0; i < children.length; i++) {
            if (children[i].get('isMissing')) {
                continue;
            }

            if (!children[i].get('isValid')) {
                return false;
            }
        }

        return true;
    }),
    toJson: function () {
        var parent = {};
        var children = this.get('children');
        for (var i = 0; i < children.length; i++) {
            var child = children[i];
            if (!child.isMissing()) {
                parent[child.get('name')] = child.toJson();
            }
        }

        var extra = this.get('extra');
        $.extend(parent, extra);
        return parent;
    },
    fromJson: function (json) {
        Utils.assert(Utils.isUndefinedOrTypeOf('object', json));
        var children = this.get('children');
        var isMissing = json === undefined;
        this.set('_isMissing', isMissing);
        if (isMissing) {
            for (var i = 0; i < children.length; i++) {
                children[i].fromJson(undefined);
            }

            return;
        }

        var seenKeys = {};
        for (var i = 0; i < children.length; i++) {
            var propName = children[i].get('name');
            children[i].fromJson(json[propName]);
            seenKeys[propName] = true;
        }

        var extra = {};
        for (var propName in json) {
            if (!seenKeys[propName]) {
                extra[propName] = json[propName];
            }
        }
        this.set('extra', extra);
    }
});