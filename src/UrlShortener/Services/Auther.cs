using System;
using Microsoft.AspNetCore.Http;

namespace UrlShortener.Services
{
    public class Auther : IAuther
    {
        readonly IConfigHelper _configHelper;
        readonly HttpContext _context;

        public Auther(IConfigHelper configHelper, IHttpContextAccessor httpContextAccessor)
        {
            _configHelper = configHelper;
            _context = httpContextAccessor.HttpContext;
        }

        public void ValidateKey(string key)
        {
            // Make sure an auth key is set in the config.
            if (string.IsNullOrEmpty(_configHelper.AuthKey))
            {
                throw new UnauthorizedAccessException("No auth key set");
            }

            // Make sure the given key matches.
            if (key != _configHelper.AuthKey)
            {
                throw new UnauthorizedAccessException("Auth key doesn't match");
            }
        }

        public bool AttemptAuth(string key)
        {
            try
            {
                // Validate key given
                ValidateKey(key);

                // If we get here, the key was fine, so process it.
                _context.Response.Cookies.Append(_configHelper.AuthKeyCookieName, key);

                // Say that the auth was successful.
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                // If we are here, the validation threw, so return failure.
                return false;
            }
        }
    }
}
