using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityServer
{
    public static class Configurationn
    {
        /// <summary>
        /// 创建身份资源
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<IdentityResource> GetIdentityResources() =>
            new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                //new IdentityResources.Profile(),
                //添加其他声明
                new IdentityResource
                {
                    Name = "rc.scope",
                    UserClaims =
                    {
                        "rc.garndma"
                    }
                }
            };

        /// <summary>
        /// 获取所有API
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ApiResource> GetApis() => new List<ApiResource> {
            new ApiResource("ApiOne"),
        };

        public static IEnumerable<ApiScope> GetApisScope() => new List<ApiScope> {
            //设定资源范围
            //在访问ApiOne资源中，Claims中包括rc.api.garndma
            //new ApiScope("ApiOne",new string[]{ "rc.api.garndma" }),
            new ApiScope("ApiOne"),
            new ApiScope("ApiTwo",new string[]{ "rc.api.garndma" }),
        };

        /// <summary>
        /// 获取所有的客户端
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Client> GetClients() =>
            new List<Client> {
                new Client
                {
                    ClientId = "client_id",
                    ClientSecrets = { new Secret("client_secret".ToSha256()) },
                    AllowedGrantTypes = GrantTypes.ClientCredentials, //访问类型(这里我们以客户凭证流)
                    AllowedScopes = { "ApiOne" }, //作用范围
                },
                new Client
                {
                    ClientId = "client_id_mvc",
                    ClientSecrets = { new Secret("client_secret_mvc".ToSha256()) },
                    AllowedGrantTypes = GrantTypes.Code, //访问类型(这里我们以Code)
                    //初始化前进行验证
                    //切换为PKCE流
                    //可以参考： https://www.cnblogs.com/xhy0826/p/12530951.html
                    RequirePkce = true,
                    RedirectUris = { "https://localhost:44341/signin-oidc" },
                    //退出时跳转的地址
                    PostLogoutRedirectUris = { "https://localhost:44341/Home/Index" },

                    AllowedScopes = {
                        "ApiOne",
                        "ApiTwo",
                        IdentityServerConstants.StandardScopes.OpenId,
                        //IdentityServerConstants.StandardScopes.Profile,
                        "rc.scope"
                    }, 
                    
                   
                    //将所有声明放入id令牌中
                    //AlwaysIncludeUserClaimsInIdToken = true,
                    
                    //作用范围
                    RequireConsent = false,

                    //允许离线访问
                    AllowOfflineAccess = true
                },
                new Client
                {
                    ClientId = "client_id_js",
                    // 为   Code前Implicit
                    AllowedGrantTypes = GrantTypes.Code,

#region 为Code后
                    //切换为PKCE流验证
                    RequirePkce = true,
                    //如果设置为false，则在令牌端点请求令牌时不需要任何客户端机密
                    RequireClientSecret = false,
#endregion
                    //登录成功后，跳转的地址
                    RedirectUris = { "https://localhost:44333/Home/signin" },
                    //退出时跳转的地址
                    PostLogoutRedirectUris = { "https://localhost:44333/Home/Index" },
                    //跨域请求白名单
                    AllowedCorsOrigins = { "https://localhost:44333" },
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        "ApiOne",
                        "ApiTwo",
                        "rc.scope"
                    },
                    //访问令牌的生命周期为1秒钟  其实要1分钟
                    AccessTokenLifetime = 1,
                    
                    //允许浏览器访问令牌
                    AllowAccessTokensViaBrowser = true,
                    //作用范围
                    RequireConsent = false
                },
                //与js差不多
                new Client {

                    ClientId = "wpf",

                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    RequireClientSecret = false,

                    RedirectUris = { "http://localhost/sample-wpf-app" },
                    AllowedCorsOrigins = { "http://localhost" },

                    AllowedScopes = {
                        IdentityServerConstants.StandardScopes.OpenId,
                        "ApiOne",
                    },

                    AllowAccessTokensViaBrowser = true,
                    RequireConsent = false,
                },
                // wpf 2 client password grant
                new Client
                {
                    ClientId = "wpf2",
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                    ClientSecrets = { new Secret("wpf secrect".Sha256()) },
                    AllowedCorsOrigins = { "http://localhost" },
                    AllowedScopes = { "ApiOne",
                        IdentityServerConstants.StandardScopes.OpenId
                    },
                    RequireConsent = false,
                },
                new Client {

                    ClientId = "xamarin",

                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    RequireClientSecret = false,

                    RedirectUris = { "xamarinformsclients://callback" },

                    AllowedScopes = {
                        IdentityServerConstants.StandardScopes.OpenId,
                        "ApiOne",
                    },

                    AllowAccessTokensViaBrowser = true,
                    RequireConsent = false,
                },

                new Client {
                    ClientId = "angular",

                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    RequireClientSecret = false,

                    RedirectUris = { "http://localhost:4200" },
                    PostLogoutRedirectUris = { "http://localhost:4200" },
                    AllowedCorsOrigins = { "http://localhost:4200" },

                    AllowedScopes = {
                        IdentityServerConstants.StandardScopes.OpenId,
                        "ApiOne",
                    },

                    AllowAccessTokensViaBrowser = true,
                    RequireConsent = false,
                },
                new Client {
                    ClientId = "flutter",

                    AllowedGrantTypes = GrantTypes.Code,
                    RequirePkce = true,
                    RequireClientSecret = false,

                    RedirectUris = { "http://localhost:4000/" },
                    AllowedCorsOrigins = { "http://localhost:4000" },

                    AllowedScopes = {
                        IdentityServerConstants.StandardScopes.OpenId,
                        "ApiOne",
                    },

                    AllowAccessTokensViaBrowser = true,
                    RequireConsent = false,
                },
            };
    }
}
