using System;
using System.Text.RegularExpressions;
using Amazon.DynamoDBv2.DocumentModel;

namespace UrlShortener.Models
{
    public class ForwardItem
    {
        public static class DbKeys
        {
            public const string Id = "forwardId";
            public const string Dest = "dest";
            public const string Notes = "notes";
            public const string Hits = "hits";
        }

        internal static void ValidateDest(string dest)
        {
            if (string.IsNullOrEmpty(dest))
            {
                throw new ValidationException("Destination cannot be empty");
            }

            if (!(Uri.TryCreate(dest, UriKind.Absolute, out Uri uri) &&
                (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)))
            {
                throw new ValidationException("Destination has to be an URL");
            }
        }

        public static ForwardItem FromDocument(Document doc)
        {
            return new ForwardItem(
                doc[DbKeys.Id].AsString(),
                doc[DbKeys.Dest].AsString(),
                doc.ContainsKey(DbKeys.Notes) ? doc[DbKeys.Notes].AsString() : "",
                doc[DbKeys.Hits].AsInt()
            );
        }

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
            if (string.IsNullOrEmpty(Id))
            {
                throw new ValidationException("Forward ID cannot be empty");
            }

            if (!Regex.IsMatch(Id, @"^[A-Za-z0-9-._~!$&'()*+,;=:@%]+$"))
            {
                throw new ValidationException("Forward ID can only contain characters valid in an URL path");
            }

            ValidateDest(Dest);
        }

        public Document ToDocument()
        {
            var doc = new Document
            {
                [DbKeys.Id] = Id,
                [DbKeys.Dest] = Dest,
                [DbKeys.Hits] = Hits
            };

            if (!string.IsNullOrEmpty(Notes))
            {
                doc[DbKeys.Notes] = Notes;
            }

            return doc;
        }
    }
}
