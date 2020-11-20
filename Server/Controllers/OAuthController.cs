using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace Server.Controllers
{
    public class OAuthController:Controller
    {
        [HttpGet]
        public IActionResult Authorize(
            string response_type, // 授权流程类型
            string client_id, // 客户端ID
            string redirect_uri, // 重定向连接地址
            string scope,// 什么是我想要的
            string state // 状态

        )
        {
            var query = new QueryBuilder();
            query.Add("redirectUri",redirect_uri);
            query.Add("state",state);
            return View(model: query.ToString());
        }

        [HttpPost]
        public IActionResult Authorize(
            string username,
            string redirectUri,
            string state)
        {
            const string code = "BABABABABABABABA";
            var query = new QueryBuilder();
            query.Add("code",code);
            query.Add("state",state);
            return Redirect($"{redirectUri}{query.ToString()}");
        }

        public async Task<IActionResult> Token(
            string grant_type, //认证access_token的请求流程
            string code, //身份认证过程的确认
            string redirect_uri, 
            string client_id,
            string refresh_token
        ){
            //验证客户端代码的方法


            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub,"some_id"),
                new Claim("granny","cookie")
            };

            //创建安全byte
            var secreBytes = Encoding.UTF8.GetBytes(Constants.Secret);
            //创建键
            var key = new SymmetricSecurityKey(secreBytes);
            //创建加密方式
            var algorithm = SecurityAlgorithms.HmacSha256;
            //初始化Token
            var signingCredentials = new SigningCredentials(key, algorithm);
            //创建JWT的实例
            var token = new JwtSecurityToken(
                Constants.Issuer,
                Constants.Audiznce,
                claims,
                notBefore: DateTime.Now,
                expires: grant_type== "refresh_token" 
                ? DateTime.Now.AddMinutes(5)
                : DateTime.Now.AddMilliseconds(1),
                signingCredentials
                );
            //获取tokenJson字符串
            var access_token = new JwtSecurityTokenHandler().WriteToken(token);

            var response_type = new {
                access_token,
                token_type="Bearer",
                raw_claim="oauthTutorial",
                refresh_token = "RefreshTokenSampleValueSomething77"
            };
            
            var responseJson = JsonConvert.SerializeObject(response_type);

            var responseBytes = Encoding.UTF8.GetBytes(responseJson);

            await Response.Body.WriteAsync(responseBytes,0,responseBytes.Length);

            return Redirect(redirect_uri);

        }
        
        [Authorize]
        public IActionResult Validate()
        {
            if (HttpContext.Request.Query.TryGetValue("access_token",out var accessToken))
            {

                return Ok();
            }
            return BadRequest();
        }
    }    
}
