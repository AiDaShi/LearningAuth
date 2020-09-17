using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace MicrosoftDefaultAuth.AuthorizationRequirements
{
    public class CustomRequireClaim:IAuthorizationRequirement
    {
        public string ClaimType { get;}

        public CustomRequireClaim(string ClaimType)
        {
            this.ClaimType = ClaimType;
        }
    }

    public class CustomRequireClaimHandle : AuthorizationHandler<CustomRequireClaim>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CustomRequireClaim requirement)
        {
            var hasClaim = context.User.Claims.Any(x => x.Type == requirement.ClaimType);
            if (hasClaim)
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}