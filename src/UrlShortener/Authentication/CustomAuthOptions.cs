using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Primitives;

namespace UrlShortener.Authentication
{
    public class CustomAuthOptions : AuthenticationSchemeOptions
    {
        public const string DefaultScheme = "UrlShortener-Custom-Auth";
        public string Scheme => DefaultScheme;
        public StringValues AuthKey { get; set; }
    }
}
