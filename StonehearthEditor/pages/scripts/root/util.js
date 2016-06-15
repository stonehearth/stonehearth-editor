var formatEffectTrack = function (json, options) {

    var formatted = JSON.stringify(json, null, 3);
    formatted = formatted.replace(/\[[^\]\[]*\]/g, function(match) {
        var ret = match.replace(/(\s|\n)/g, "").replace(/,/g, ", ");
        return ret;
    });
    return formatted;
};