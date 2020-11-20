using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace MvcClient.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public IActionResult Index() 
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> Secret() 
        {
            //获取token
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var idToken = await HttpContext.GetTokenAsync("id_token");
            //需要添加脱机访问令牌(offline_access)的声明，添加identityServer添加脱机允许
            var refreshToken = await HttpContext.GetTokenAsync("refresh_token");

            var claims = User.Claims.ToList();
            var _accessToken = new JwtSecurityTokenHandler().ReadJwtToken(accessToken);
            var _idToken = new JwtSecurityTokenHandler().ReadJwtToken(idToken);

            var result = await GetSecret(accessToken);

            await RefreshAccessToken();
            return View();
        }

        public IActionResult Logout()
        {
            return SignOut("Cookie", "oidc");
        }

        /// <summary>
        /// 获取ApiOne的案例
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public async Task<string> GetSecret(string accessToken) 
        {
            var apiClient = _httpClientFactory.CreateClient();
            apiClient.SetBearerToken(accessToken);
            var response = await apiClient.GetAsync("https://localhost:44369/secret");
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }

        /// <summary>
        /// 刷新令牌
        /// </summary>
        /// <returns></returns>
        private async Task RefreshAccessToken()
        {
            var serverClient = _httpClientFactory.CreateClient();
            var discoveryDocument = await serverClient.GetDiscoveryDocumentAsync("https://localhost:44376/");

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var refresh_token = await HttpContext.GetTokenAsync("refresh_token");
            var idToken = await HttpContext.GetTokenAsync("id_token");
            var refreshTokenClient = _httpClientFactory.CreateClient();

            //请求Token
            var tokenResponse = await refreshTokenClient.RequestRefreshTokenAsync(new RefreshTokenRequest() { 
                Address = discoveryDocument.TokenEndpoint,
                RefreshToken = refresh_token,
                ClientId = "client_id_mvc",
                ClientSecret = "client_secret_mvc"
            });
            
            //下面将修改上下文
            var authInfo = await HttpContext.AuthenticateAsync("Cookie");

            authInfo.Properties.UpdateTokenValue("access_token", tokenResponse.AccessToken);
            authInfo.Properties.UpdateTokenValue("id_token", tokenResponse.IdentityToken);
            authInfo.Properties.UpdateTokenValue("refresh_token", tokenResponse.RefreshToken);

            //二次认证（更新token）
            await HttpContext.SignInAsync("Cookie", authInfo.Principal, authInfo.Properties);

            //验证Token是否不同 三个都是不同的true
            var accessTokenDifferent = !accessToken.Equals(tokenResponse.AccessToken);
            var idTokenDifferent = !accessToken.Equals(tokenResponse.IdentityToken);
            var refreshTokenDifferent = !accessToken.Equals(tokenResponse.RefreshToken);
        }
    }
}