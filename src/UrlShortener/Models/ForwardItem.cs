using System;

namespace UrlShortener.Models
{
    public class ForwardItem
    {
        public string id { get; set; }
        public string Dest { get; set; }
        public string Note { get; set; }
        public int Hits { get; set; }

        public ForwardItem(string id, string dest, string note = "", int? hits = null)
        {
            this.id = id;
            Dest = dest;
            Note = note;
            Hits = hits ?? 0;
        }
    }
}
