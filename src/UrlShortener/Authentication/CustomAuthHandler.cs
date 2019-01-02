using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using UrlShortener.Services;

namespace UrlShortener.Authentication
{
    public class CustomAuthHandler : AuthenticationHandler<CustomAuthOptions>
    {
        readonly IConfigHelper _configHelper;
        readonly IAuther _auther;

        public CustomAuthHandler(
            IOptionsMonitor<CustomAuthOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IConfigHelper configHelper,
            IAuther auther)
            : base(options, logger, encoder, clock)
        {
            _configHelper = configHelper;
            _auther = auther;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            try
            {
                // Make sure the cookie was actually given.
                if (!Request.Cookies.ContainsKey(_configHelper.AuthKeyCookieName))
                {
                    throw new UnauthorizedAccessException("No auth key cookie given");
                }

                // Validate given key.
                _auther.ValidateKey(Request.Cookies[_configHelper.AuthKeyCookieName]);

                // Create authenticated user
                var identities = new List<ClaimsIdentity>
                {
                    new ClaimsIdentity(new List<Claim> {
                        new Claim(ClaimTypes.Name, "admin", ClaimValueTypes.String)
                    }, "auth-key")
                };
                var ticket = new AuthenticationTicket(new ClaimsPrincipal(identities), Options.Scheme);

                return Task.FromResult(AuthenticateResult.Success(ticket));
            }
            catch (UnauthorizedAccessException ex)
            {
                return Task.FromResult(AuthenticateResult.Fail(ex.Message));
            }
        }
    }
}
