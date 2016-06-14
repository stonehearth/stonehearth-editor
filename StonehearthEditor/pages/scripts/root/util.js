var formatEffectTrack = function (json, options) {

    var formatted = JSON.stringify(json, null, 3);
    formatted = formatted.replace(/(\[)[\s\n]*([\d\.-]+)[\s\n]*(\])/g, '$1$2$3')
    formatted = formatted.replace(/(\[)[\s\n]*([\d\.-]+)[\s\n]*,[\s\n]*([\d\.-]+)[\s\n]*\]/g, '$1$2, $3]')
    formatted = formatted.replace(/\[[\s\n]*([\d\.-]+)[\s\n]*,[\s\n]*([\d\.-]+)[\s\n]*,[\s\n]*([\d\.-]+)[\s\n]*,[\s\n]*([\d\.-]+)[\s\n]*\]/g, '[$1, $2, $3, $4]')
    return formatted;
};