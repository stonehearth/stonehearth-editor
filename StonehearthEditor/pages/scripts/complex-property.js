ComplexProperty = EffectProperty.extend({
    componentName: 'complex-property',
    children: null, // list of EffectProperty
    optional: null, // bool
    extra: null, // dictionary of extra fields
    isMissing: null, // bool, set internally
    isValid: Ember.computed('children', 'children.@each.isValid', function () {
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
            if (!child.get('isMissing')) {
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
        var isOptional = !!this.get('optional');

        // Here we expect required fields to have reasonable defaults.
        this.set('isMissing', isOptional ? isMissing : false);

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
    },
    isRoot: Ember.computed('name', function () {
        return this.get('name') === null;
    }),
});