var createState = function () {
    return "CfDJ8BfVIH88qX5KpcBl1RIBWCrEGjVs0UQhx30lLC1XDagWqc5n6LBePXduIUP6iGejvHpGqE3GO3-jutkDWRcyROkUoMhftrVFwB0caJLGg2MqUKLdZW03WJ8_vT0D1Q4d3wu9zZeVXAgIEXc2OGGOJRztyw1HX843cLlTNBmptQTh_YY1u9YvZmjIY3KP72azusEk-3cyF53T5Sw7oxc_o9UMF29dzTmMa4TgV3eBUn51p2PrcSQBVg3047ZS5jiHV_Vn4wCNmYqGf813LMIncgIdZqDAjUwPaNhxPa45KSQr16R00MXZ6Dp-2ny3Edj38o_SNInA43_u5nZQR1rPk6QI5lkURM6gafPb6iw8e-jKFeJcQLxd47B1fGPR5A24qw";
}

var createNonce = function () {
    return "637399948254998276.NjZlOGQzNzAtM2MwNy00MzZkLTgyNTMtNjBjMzk4MjkwYzYzMDRlZmI4OTAtYTUyYy00MzVmLThjZjItNjA1YjdlMzcwN2Vk";
}

var SignIn = function () {
    var redirectUri = "https://localhost:44333/Home/SignIn";
    var responseType = "id_token token";
    var scope = "openid ApiOne";
    var authUrl = "/connect/authorize/callback"+
"?client_id=client_id_js"+
        "&redirect_uri="+encodeURIComponent(redirectUri) +
        "&response_type="+encodeURIComponent(responseType) +
        "&scope=" + encodeURIComponent(scope)+
        "&nonce=" +createNonce() +
        "&state=" +createState(); 

    var returnUrl = encodeURIComponent(authUrl);
    console.log(authUrl);
    console.log(returnUrl);

    window.location.href = "https://localhost:44376/Auth/Login?ReturnUrl=" + returnUrl;
}