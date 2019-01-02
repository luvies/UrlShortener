using System;

namespace UrlShortener.Models
{
    public class ForwardItemUpdate
    {
        public string Dest { get; set; }
        public string Notes { get; set; }

        public void Validate()
        {
            if (string.IsNullOrEmpty(Dest))
            {
                throw new ArgumentException();
            }
        }
    }
}
