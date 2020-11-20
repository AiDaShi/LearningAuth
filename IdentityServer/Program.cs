﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace IdentityServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var userManage = scope.ServiceProvider
                    .GetRequiredService<UserManager<IdentityUser>>();
                //创建用户
                var user = new IdentityUser("bob");
                userManage.CreateAsync(user, "password").GetAwaiter().GetResult();
                //添加其他声明
                userManage.AddClaimAsync(user, new Claim("rc.garndma", "big.cookie")).GetAwaiter().GetResult();
                userManage.AddClaimAsync(user, new Claim("rc.api.garndma", "big.api.cookie")).GetAwaiter().GetResult();

                #region 第一次的时候需要取消这段代码的注释
                ////下面是官网复制的
                ////加入数据库到id4中
                //scope.ServiceProvider.GetRequiredService<PersistedGrantDbContext>().Database.Migrate();

                //var context = scope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
                ////迁移一下
                ////context.Database.Migrate();
                //////添加 客户端，角色，资源
                //if (!context.Clients.Any())
                //{
                //    foreach (var client in Configurationn.GetClients())
                //    {
                //        context.Clients.Add(client.ToEntity());
                //    }
                //    context.SaveChanges();
                //}

                //if (!context.IdentityResources.Any())
                //{
                //    foreach (var resource in Configurationn.GetIdentityResources())
                //    {
                //        context.IdentityResources.Add(resource.ToEntity());
                //    }
                //    context.SaveChanges();
                //}

                //if (!context.ApiScopes.Any())
                //{
                //    foreach (var resource in Configurationn.GetApisScope())
                //    {
                //        context.ApiScopes.Add(resource.ToEntity());
                //    }
                //    context.SaveChanges();
                //}
                #endregion
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}