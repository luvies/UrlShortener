using System;
using Microsoft.Extensions.Configuration;

namespace UrlShortener.Services
{
    public class ConfigHelper : IConfigHelper
    {
        const string Key_AppTable = "AppTable";
        const string Key_AuthKey = "AuthKey";
        const string CookieName_AuthKey = "auth-token";

        readonly IConfiguration _configuration;

        public ConfigHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string AppTable => _configuration[Key_AppTable];

        public string AuthKey => _configuration[Key_AuthKey];

        public string AuthKeyCookieName => CookieName_AuthKey;
    }
}
