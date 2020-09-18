using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MicrosoftDefaultAuth.AuthorizationRequirements;
using MicrosoftDefaultAuth.Controllers;
using MicrosoftDefaultAuth.CustomAuthorizationPolicyProvider;
using MicrosoftDefaultAuth.Extensions;
using MicrosoftDefaultAuth.Transformer;

namespace MicrosoftDefaultAuth
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
            services.AddAuthentication("CookieAuth")
                .AddCookie("CookieAuth",config=>{
                    config.Cookie.Name = "HeMinYang";
                    config.LoginPath = "/Home/Index";
                });

            services.AddSingleton<IAuthorizationPolicyProvider, CustomAuthorizationPolicyProvider.CustomAuthorizationPolicyProvider>();
            services.AddScoped<IAuthorizationHandler, SecurityLevelHandler>();
            services.AddScoped<IAuthorizationHandler, CustomRequireClaimHandle>();
            services.AddScoped<IAuthorizationHandler, CookieJarAuthorizationHandler>();
            services.AddScoped<IClaimsTransformation, ClaimsTransformation>();

            //因为有了CustomAuthorizationPolicyProvider的依赖注入在这里我们就不需要这个了
            services.AddAuthorization(config =>
            {
                //第一种
                //创建构建器
                //var defaultAuthBuilder = new AuthorizationPolicyBuilder();
                //var defaultAuthPolicy = defaultAuthBuilder
                //    //需要有用户
                //    .RequireAuthenticatedUser()
                //    //需要有出生日期
                //    .RequireClaim(ClaimTypes.DateOfBirth)
                //    .Build();
                //config.DefaultPolicy = defaultAuthPolicy;

                //第二种
                //config.AddPolicy("Claim.DoB", policyBuilder => {
                //    policyBuilder.RequireClaim(ClaimTypes.DateOfBirth);
                //});

                //第三种 注册授权政策
                //config.AddPolicy("Claim.DoB", policyBuilder => {
                //    policyBuilder.AddRequirements(new CustomRequireClaim(ClaimTypes.DateOfBirth));
                //});

                //第四种 （中间件的方式）
                config.AddPolicy("Claim.DoB", policyBuilder =>
                {
                    policyBuilder.RequireCustomClaim(ClaimTypes.DateOfBirth);
                });

                config.AddPolicy("Admin", policyBuilder => policyBuilder.RequireClaim(ClaimTypes.Role, "Admin"));

            });


            services.AddControllersWithViews(config=> {

                var defaultAuthBuilder = new AuthorizationPolicyBuilder();
                var defaultAuthPolicy = defaultAuthBuilder
                    //需要有用户
                    .RequireAuthenticatedUser()
                    //需要有出生日期
                    .RequireClaim(ClaimTypes.DateOfBirth)
                    .Build();

                //config.Filters.Add(new AuthorizeFilter(defaultAuthPolicy));
            });

            //添加Razor服务
            services.AddRazorPages()
                .AddRazorPagesOptions(config=> {
                    config.Conventions.AuthorizePage("/Razor/Secured");
                    //验证路径 与 政策
                    config.Conventions.AuthorizePage("/Razor/Policy","Admin");
                    //添加文件夹路由
                    config.Conventions.AuthorizeFolder("/RazorSecured");
                    config.Conventions.AllowAnonymousToPage("/RazorSecured/Anon");
                });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseStaticFiles();
            app.UseRouting();
            //who are you
            app.UseAuthentication();
            //are you allowed
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                //调用Razor中间件
                endpoints.MapRazorPages();
            });
        }
    }
}
