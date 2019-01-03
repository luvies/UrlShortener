using System;

namespace UrlShortener.Services
{
    public interface IConfigHelper
    {
        string AppTable { get; }
        string AuthKey { get; }
        string CurrentOrigin { get; }
        string AuthKeyCookieName { get; }
    }
}
