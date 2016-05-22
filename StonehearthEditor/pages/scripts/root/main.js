console.log("main.js");

Utils = {
    isUndefinedOrTypeOf: function (type, obj) {
        return typeof (obj) === type || typeof (obj) === undefined;
    },
    assert: function (condition, message) {
        if (!condition) {
            throw "AssertionError" + (message || "");
        }
    }

};