using System;
using Microsoft.AspNetCore.Http;

namespace UrlShortener.Services
{
    public interface IAuther
    {
        void ValidateKey(string key);
        bool AttemptAuth(string key);
    }
}
