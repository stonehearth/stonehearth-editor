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
    formatEffectJson: function (json, options) {

        var formatted = JSON.stringify(json, null, 3);
        formatted = formatted.replace(/\[[^\]\[]*\]/g, function (match) {
            var ret = match.replace(/(\s|\n)/g, "").replace(/,/g, ", ");
            return ret;
        });
        return formatted;
    },
    getEffectValueOrDefault: function (json, index, defaultVal) {
        if (json && json[index]) {
            return json[index].toString();
        } else {
            return defaultVal;
        }
    }
};