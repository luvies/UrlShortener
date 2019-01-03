using System;
using Amazon.DynamoDBv2.DocumentModel;

namespace UrlShortener.Models
{
    public class ForwardItemUpdate
    {
        public string Dest { get; set; }
        public string Notes { get; set; }

        public void Validate()
        {
            ForwardItem.ValidateDest(Dest);
        }

        public Document ToDocument(string id)
        {
            var doc = new Document
            {
                [ForwardItem.DbKeys.Id] = id,
                [ForwardItem.DbKeys.Dest] = Dest
            };

            if (!string.IsNullOrEmpty(Notes))
            {
                doc[ForwardItem.DbKeys.Notes] = Notes;
            }

            return doc;
        }
    }
}
