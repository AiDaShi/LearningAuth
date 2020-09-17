using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MicrosoftDefaultAuth.CustomAuthorizationPolicyProvider
{

    public class SecurityLevelAttribute : AuthorizeAttribute
    {
        public SecurityLevelAttribute(int level)
        {
            Policy = $"{DynamicPilicies.SecurityLevel}.{level}";
        }
    }

    public class Dummy
    {
        public Dummy()
        {
        }
    }

    // {type}
    public static class DynamicPilicies
    {
        public static IEnumerable<string> Get()
        {
            yield return SecurityLevel;
            yield return Rank;
        }

        public const string SecurityLevel = "SecurityLevel";
        public const string Rank = "Rank";
    }

    public static class DynamicAuthorizationPolicyFactory
    {
        public static AuthorizationPolicy Create(string policyName) 
        {
            var parts = policyName.Split('.');
            var type = parts.First();
            var value = parts.Last();

            switch (type)
            {
                case DynamicPilicies.Rank:
                    return new AuthorizationPolicyBuilder()
                        .RequireClaim("Rank", value)
                        .Build();
                case DynamicPilicies.SecurityLevel:
                    return new AuthorizationPolicyBuilder()
                        .AddRequirements(new SecurityLevelRequirement(Convert.ToInt32(value)))
                        .Build();
                default:
                    return null;
            }
        }
    }

    /// <summary>
    /// 对自定义的条件政策进行处理
    /// </summary>
    public class SecurityLevelHandler : AuthorizationHandler<SecurityLevelRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, SecurityLevelRequirement requirement)
        {
            var calimValue = Convert.ToInt32(context.User.Claims
                .FirstOrDefault(x=>x.Type == DynamicPilicies.SecurityLevel)
                ?.Value ?? "0");
            //判断Claims中的SecurityLevel是否大于初始化时的定义
            if (requirement.Level>= calimValue)
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }

    /// <summary>
    /// 自定义条件政策
    /// </summary>
    public class SecurityLevelRequirement : IAuthorizationRequirement
    {
        public int Level { get; set; }
        public SecurityLevelRequirement(int level) 
        {
            Level = level;
        }
    }


    public class CustomAuthorizationPolicyProvider
        : DefaultAuthorizationPolicyProvider
    {
        public CustomAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options) : base(options)
        {

        }

        // {type}.{value} 选择政策器
        public override Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            foreach (var item in DynamicPilicies.Get())
            {
                //通过判断政策名称进行筛选
                if (policyName.StartsWith(item))
                {
                    //使用工厂的模式进行创建
                    var policy = DynamicAuthorizationPolicyFactory.Create(policyName);
                    return Task.FromResult(policy);
                }
            }
            return base.GetPolicyAsync(policyName);
        }
    }
}
