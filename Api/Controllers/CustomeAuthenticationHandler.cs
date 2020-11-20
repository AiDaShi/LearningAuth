using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Api.Controllers
{
    public class CustomeAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public CustomeAuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock systemClock
            ):base(options, logger, encoder, systemClock)
        {
        
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {


            return Task.FromResult(AuthenticateResult.Fail("Failed Authentication"));
        }
    }
}
