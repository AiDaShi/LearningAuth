using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace Client
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(config=>{
                //我们检查cookie以确认我们已通过身份验证
                config.DefaultAuthenticateScheme = "ClientCookie";
                //当我们登录时，我们将分发一个cookie
                config.DefaultSignInScheme = "ClientCookie";
                //用这个来检查我们是否被允许做一些事情
                config.DefaultChallengeScheme = "OurServer";
            })
                .AddCookie("ClientCookie")
                .AddOAuth("OurServer",config=>{
                    
                    config.ClientId = "client_id";
                    config.ClientSecret = "client_secret";
                    config.CallbackPath = "/oauth/callback";
                    //配置授权端点
                    config.AuthorizationEndpoint= "https://localhost:44358/oauth/Authorize";
                    //客户端向服务器端发起端请求
                    config.TokenEndpoint= "https://localhost:44358/oauth/token";
                    config.SaveTokens = true;
                    config.Events = new OAuthEvents()
                    {
                        OnCreatingTicket = context =>
                        {
                            //验证通过后触发事件
                            var accessToken =context.AccessToken;
                            
                            var basepayload = accessToken.Split('.')[1];
                            //base64获取有问题

                            //var bytes = Convert.FromBase64String(basepayload);
                            //var jsonPayload = Encoding.UTF8.GetString(bytes);
                            var jsonPayload = Base64UrlEncoder.Decode(basepayload);
                            var claims = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonPayload);

                            foreach (var item in claims)
                            {
                                context.Identity.AddClaim(new Claim(item.Key,item.Value));
                            }
                            return Task.CompletedTask;
                        }
                    };
                });

            services.AddHttpClient();
            
            services.AddControllersWithViews()
            .AddRazorRuntimeCompilation();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
