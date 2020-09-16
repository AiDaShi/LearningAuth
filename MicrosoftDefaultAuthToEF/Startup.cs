using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MicrosoftDefaultAuthToEF.Data;
using NETCore.MailKit.Extensions;
using NETCore.MailKit.Infrastructure.Internal;

namespace MicrosoftDefaultAuthToEF
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
            services.AddDbContext<AppDbContext>(config=>{
                //将数据存入本地内存中
                config.UseInMemoryDatabase("Memory");
            });
            
            services.AddIdentity<IdentityUser,IdentityRole>(config=>{
                config.Password.RequiredLength = 4;
                config.Password.RequireDigit = false;
                config.Password.RequireNonAlphanumeric = false;
                config.Password.RequireUppercase = false;
                config.SignIn.RequireConfirmedEmail = true;
                
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders(); //重置密码和Token程序

            services.ConfigureApplicationCookie(config=>{
                config.Cookie.Name = "Identity.Cookie";
                config.LoginPath = "/Home/Login";
            });
            

            // services.AddAuthentication("CookieAuth")
            //     .AddCookie("CookieAuth",config=>{
            //         config.Cookie.Name = "HiMinYang";
            //         config.LoginPath = "/Home/Index";
            //     });

            services.AddControllersWithViews();
            //Add Email Server Address
            var EmailConfig = Configuration.GetSection("Email").Get<MailKitOptions>();
            services.AddMailKit(config=>config.UseMailKit(Configuration.GetSection("Email").Get<MailKitOptions>()));

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

            //who are you
            app.UseAuthentication();
            //are you allowed
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
