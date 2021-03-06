﻿using System;
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
            public const string CreatedAt = "createdAt";
            public const string UpdatedAt = "updatedAt";
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
                doc[DbKeys.Hits].AsInt(),
                DateTime.Parse(doc[DbKeys.CreatedAt].AsString()),
                DateTime.Parse(doc[DbKeys.UpdatedAt].AsString())
            );
        }

        public string Id { get; set; }
        public string Dest { get; set; }
        public string Notes { get; set; }
        public int Hits { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ForwardItem()
        {
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public ForwardItem(
            string id, string dest, string notes = "",
            int? hits = null, DateTime? createdAt = null,
            DateTime? updatedAt = null) : this()
        {
            Id = id;
            Dest = dest;
            Notes = notes;
            Hits = hits ?? 0;
            CreatedAt = createdAt ?? CreatedAt;
            UpdatedAt = updatedAt ?? UpdatedAt;
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
            return new Document
            {
                [DbKeys.Id] = Id,
                [DbKeys.Dest] = Dest,
                [DbKeys.Notes] = Notes ?? "",
                [DbKeys.Hits] = Hits,
                [DbKeys.CreatedAt] = CreatedAt.ToString("o"),
                [DbKeys.UpdatedAt] = UpdatedAt.ToString("o")
            };
        }
    }
}
