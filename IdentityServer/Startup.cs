using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using IdentityServer.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IdentityServer
{
    public class Startup
    {
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _env;
        public Startup(IConfiguration config, IWebHostEnvironment env) 
        {
            _config = config;
            _env = env;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = _config.GetConnectionString("DefaultConnection");

            services.AddDbContext<AppDbContext>(config => {
                //将数据存入本地内存中
                //config.UseInMemoryDatabase("Memory");
                //存入到sqlserver中
                config.UseSqlServer(connectionString);
            });

            services.AddIdentity<IdentityUser, IdentityRole>(config => {
                config.Password.RequiredLength = 4;
                config.Password.RequireDigit = false;
                config.Password.RequireNonAlphanumeric = false;
                config.Password.RequireUppercase = false;

            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders(); //重置密码和Token程序

            services.ConfigureApplicationCookie(config => {
                config.Cookie.Name = "IdentityServer.Cookie";
                config.LoginPath = "/Auth/Login";
                config.LogoutPath = "/Auth/Logout";
            });

            //Config https://localhost:44376/.well-known/openid-configuration

            var assembly = typeof(Startup).Assembly.GetName().Name;

            #region 自定义的签名凭证

            //var certFilePath = Path.Combine(_env.ContentRootPath, "is_cert.pfx");
            //var certificate = new X509Certificate2(certFilePath,"123456789");

            #endregion

            //迁移identity4至数据库中
            //services.AddIdentityServer()
            //    //验证cookie设置
            //    .AddAspNetIdentity<IdentityUser>()
            //    .AddConfigurationStore(options =>
            //    {
            //        options.ConfigureDbContext = b => b.UseSqlServer(connectionString,
            //            sql => sql.MigrationsAssembly(assembly));
            //    })
            //    .AddOperationalStore(options =>
            //    {
            //        options.ConfigureDbContext = b => b.UseSqlServer(connectionString,
            //            sql => sql.MigrationsAssembly(assembly));
            //    })
            //    //开发人员签证凭证
            //    .AddDeveloperSigningCredential()
            //    //添加一个自定义的签名凭证：生成请参考 Certificate2_PowerShell_Command.txt
            //    //.AddSigningCredential(certificate)
            //    ;

            // 原来的 old 可以作用于 JavaScript 客户端以及 MVC 客户端
            // 使用的是内存数据库   测试环境
            services.AddIdentityServer()
                //验证cookie设置
                .AddAspNetIdentity<IdentityUser>()
                //添加身份资源
                .AddInMemoryIdentityResources(Configurationn.GetIdentityResources())
                //添加api
                .AddInMemoryApiScopes(Configurationn.GetApisScope())
                //.AddInMemoryApiResources(Configurationn.GetApis())
                //添加客户端
                .AddInMemoryClients(Configurationn.GetClients())
                //开发人员签证凭证
                .AddDeveloperSigningCredential()
                ;

            services.AddAuthentication()
                // 添加Facebook的认证
                .AddFacebook(config => {
                    config.AppId = "asdfasdfasv";
                    config.AppSecret = "secret";
                });

            services.AddControllersWithViews();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseIdentityServer();

            if (_env.IsDevelopment())
            {
                app.UseCookiePolicy(new CookiePolicyOptions()
                {
                    //影响cookie的相同站点属性。
                    MinimumSameSitePolicy = SameSiteMode.Lax
                });
            }

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
