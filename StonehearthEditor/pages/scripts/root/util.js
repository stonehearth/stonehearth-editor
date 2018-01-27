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
        if (json && json[index] !== undefined) {
            return json[index].toString();
        } else {
            return defaultVal;
        }
    },
    convertRgbaToFloat: function (r, g, b, a) {
        var convertNum = function (num) {
            return Math.round((num / 255) * 1000)/1000
        };
        var red = r <= 1 ? r : convertNum(r);
        var green = g <= 1 ? g : convertNum(g);
        var blue = b <= 1 ? b : convertNum(b);
        return {
            r: red.toString(),
            g: green.toString(),
            b: blue.toString(),
            a: a.toString()
        }
    },
    convertFloatToRgba: function(r, g, b, a) {
        var convert = function (num) {
            return Math.round(num * 255).toString();
        };
        return "rgba(" + convert(r) + "," + convert(g) + "," + convert(b) + "," + a + ")";
    }
};