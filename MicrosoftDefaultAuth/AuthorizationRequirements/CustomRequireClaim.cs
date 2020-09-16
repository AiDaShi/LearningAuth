using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace MicrosoftDefaultAuth.AuthorizationRequirements
{
    public class CustomRequireClaim:IAuthorizationRequirement
    {
        public string claimType {get;}

        public CustomRequireClaim(string claimType)
        {
            this.claimType = claimType;
        }
    }

    public class CustomRequireClaimHandle : AuthorizationHandler<CustomRequireClaim>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CustomRequireClaim requirement)
        {
            throw new NotImplementedException();
        }
    }

}