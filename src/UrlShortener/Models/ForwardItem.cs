using System;

namespace UrlShortener.Models
{
    public class ForwardItem
    {
        public string Id { get; set; }
        public string Dest { get; set; }
        public string Notes { get; set; }
        public int Hits { get; set; }

        public ForwardItem()
        { }

        public ForwardItem(string id, string dest, string notes = "", int? hits = null)
        {
            Id = id;
            Dest = dest;
            Notes = notes;
            Hits = hits ?? 0;
        }

        public void Validate()
        {
            if (string.IsNullOrEmpty(Id) ||
                string.IsNullOrEmpty(Dest))
            {
                throw new ArgumentException();
            }
        }
    }
}
