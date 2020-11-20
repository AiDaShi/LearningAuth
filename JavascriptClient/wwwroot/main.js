var config = {
    userStore: new Oidc.WebStorageStateStore({ store: window.localStorage }), // 用于为当前经过身份验证的用户保留用户的存储对象到localstorage 好处就是(当你的浏览器)
    authority: "https://localhost:44376/", // OIDC/OAuth2提供程序的URL。
    client_id: "client_id_js", // 在 OIDC/OAuth2 提供程序中注册的客户端应用程序的标识符。
    //response_type: "id_token token", // 默认值'id_token' OIDC/OAuth2 提供程序所需的响应类型。 Type为Implicit时
    response_type: "code", // Type为Code时
    redirect_uri: "https://localhost:44333/Home/SignIn", // 您的客户端应用程序的重定向URI，以接收来自 OIDC/OAuth2 提供程序的响应。
    post_logout_redirect_uri: "https://localhost:44333/Home/Index", // 注销后重定向的地址
    scope: "openid rc.scope ApiOne ApiTwo" // 从 OIDC/OAuth2 提供程序请求的范围。
}

// 创建客户端实例
var userManger = new Oidc.UserManager(config);


var signIn = function () {
    // 用户登录
    userManger.signinRedirect();
}

var signOut = function () {
    userManger.signoutRedirect();
}

userManger.getUser().then(user => {
    console.log("user:", user);
    if (user) {
        // 把Token放入到axios请求中去
        axios.defaults.headers.common['Authorization'] = "Bearer " + user.access_token;
    }
})

// 请求接口
var callApi = function () {
    axios.get('https://localhost:44369/secret').then(res => {
        console.log(res);
    });
}

var refreshing = false;

//输出拦截器
axios.interceptors.response.use(
    function (response) {
        return response;
    },
    function (error) {
        console.log(" axios error: ", error.response);
        let axiosConfig = error.response.config;
        // 如果状态码是401我们将会刷新令牌
        if (error.response.status === 401) {
            console.log(" axios error 401");
            // 如果已经刷新，请不要再提出其他需求
            if (!refreshing) {
                console.log("开始刷新");
                refreshing = true;

                // 刷新令牌方法
                return userManger.signinSilent().then(res => {
                    console.log(res);
                    // 更新请求Token与本地客户端的Token
                    axios.defaults.headers.common['Authorization'] = "Bearer " + res.access_token;
                    axiosConfig.headers['Authorization'] = "Bearer " + res.access_token;
                    // 并重试一下axios的请求
                    return axios(axiosConfig);
                });
            }
        }
        // 带一个有拒绝原因的方法
        return Promise.reject(error);
    });
