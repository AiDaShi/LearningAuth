var extractTokens = function (callbackUrl) {
    var returnValue = callbackUrl.split('#')[1];
    var values = returnValue.split('&');
    for (var i = 0; i < values.length; i++) {
        var v = values[i];
        var kvPair = v.split('=');
        localStorage.setItem(kvPair[0], kvPair[1]);
    }

    window.location.href = '/home/index';
}
var _callbackUrl = window.location.href;
extractTokens(_callbackUrl)