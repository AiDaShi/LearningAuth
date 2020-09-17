using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MicrosoftDefaultAuth.Transformer
{
    public class ClaimsTransformation : IClaimsTransformation
    {
        /// <summary>
        /// 这里是当有用户对授权有操作时，请求的方法执行完后执行
        /// </summary>
        /// <param name="principal"></param>
        /// <returns></returns>
        public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            var hasFriendClaim = principal.Claims.Any(x => x.Type == "Firend");
            if (!hasFriendClaim)
            {
                ((ClaimsIdentity)principal.Identity).AddClaim(new Claim("Friend", "Bob"));
            }
            return Task.FromResult(principal);
        }
    }
}
