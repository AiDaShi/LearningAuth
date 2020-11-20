using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Client.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace Client.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(
            ILogger<HomeController> logger,
            IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult Index()
        {
            return View();
        }
        //http://localhost:5001/Home/Secret
        [Authorize]
        public async Task<IActionResult> SecretAsync()
        {
            var serverResponse = await AccessTokenRefreshWrapper(
                () => SecuredGetRequest("https://localhost:44358/secret/index"));


            var apiResponse = await AccessTokenRefreshWrapper(
                () => SecuredGetRequest("https://localhost:44301/secret/index"));

            //old
            ////获取 access_token 
            //var token = await HttpContext.GetTokenAsync("access_token");
            ////获取 刷新令牌 
            //var refresh_token = await HttpContext.GetTokenAsync("refresh_token");
            ////创建请求
            //var _serverClient = _httpClientFactory.CreateClient();


            //_serverClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            //var serverResponseHeader = await _serverClient.GetAsync("https://localhost:44358/secret/index");

            ////刷新令牌
            //await RefreshAccessToken();

            //token = await HttpContext.GetTokenAsync("access_token");

            ////二次获取
            //var _apiClient = _httpClientFactory.CreateClient();

            //_apiClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            //serverResponseHeader = await _apiClient.GetAsync("https://localhost:44358/secret/index");

            //var apiResponse = await _apiClient.GetAsync("https://localhost:44301/secret/index");

            return View();
        }

        private async Task<HttpResponseMessage> SecuredGetRequest(string url)
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            //二次获取
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            return await client.GetAsync(url);
        }

        public async Task<HttpResponseMessage> AccessTokenRefreshWrapper(
            Func<Task<HttpResponseMessage>> initialRequest)
        {
            var initresponse = await initialRequest();

            if (initresponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                await RefreshAccessToken();
                initresponse = await initialRequest();
            }

            return initresponse;
        }

        private async Task RefreshAccessToken() 
        {
            var refresh_token = await HttpContext.GetTokenAsync("refresh_token");

            var refreshTokenClient = _httpClientFactory.CreateClient();

            var data = new Dictionary<string, string>()
            {
                ["grant_type"] = "refresh_token",
                ["refresh_token"] = refresh_token
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "https://localhost:44358/oauth/token")
            {
                Content = new FormUrlEncodedContent(data)
            };


            var basicCredentials = "username:password";
            var encodedCredentials = Encoding.UTF8.GetBytes(basicCredentials);
            var base64Credentials = Convert.ToBase64String(encodedCredentials);

            request.Headers.Add("Authorization", $"Basic {base64Credentials}");
            //请求Token
            var response = await refreshTokenClient.SendAsync(request);

            //反序列化
            var responseString = await response.Content.ReadAsStringAsync();
            var responseData = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseString);

            //获取新的令牌
            var newAccessToken = responseData.GetValueOrDefault("access_token");
            var newRefreshToken = responseData.GetValueOrDefault("refresh_token");

            //下面将修改上下文
            var authInfo = await HttpContext.AuthenticateAsync("ClientCookie");

            authInfo.Properties.UpdateTokenValue("access_token", newAccessToken);
            authInfo.Properties.UpdateTokenValue("refresh_token", newRefreshToken);

            //二次认证（更新token）
            await HttpContext.SignInAsync("ClientCookie", authInfo.Principal, authInfo.Properties);
        }
    }
}
