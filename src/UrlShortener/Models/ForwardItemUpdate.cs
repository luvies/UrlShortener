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
            return new Document
            {
                [ForwardItem.DbKeys.Id] = id,
                [ForwardItem.DbKeys.Dest] = Dest,
                [ForwardItem.DbKeys.Notes] = Notes ?? ""
            };
        }
    }
}
