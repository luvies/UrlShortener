using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using UrlShortener.Models;

using FwdKeys = UrlShortener.Models.ForwardItem.DbKeys;

#pragma warning disable RECS0030 // Suggests using the class declaring a static function when calling it
namespace UrlShortener.Services
{
    public class ForwardDb : IForwardDb
    {
        static readonly List<string> AllAttrs = new List<string>
        {
            FwdKeys.Id,
            FwdKeys.Dest,
            FwdKeys.Notes,
            FwdKeys.Hits,
            FwdKeys.CreatedAt,
            FwdKeys.UpdatedAt
        };

        readonly IAmazonDynamoDB _dynamoDb;
        readonly IConfigHelper _configHelper;
        readonly Table _table;

        public ForwardDb(IAmazonDynamoDB dynamoDb, IConfigHelper configHelper)
        {
            _dynamoDb = dynamoDb;
            _configHelper = configHelper;
            _table = Table.LoadTable(dynamoDb, configHelper.AppTable);
        }

        public async Task AddForward(ForwardItem forward)
        {
            forward.Validate();
            forward.Hits = 0; // Ensure hits is set to 0.

            var doc = forward.ToDocument();

            try
            {
                await _table.PutItemAsync(doc, new PutItemOperationConfig
                {
                    ConditionalExpression = new Expression
                    {
                        ExpressionStatement = "NOT #id = :id",
                        ExpressionAttributeNames = new Dictionary<string, string>
                        {
                            ["#id"] = FwdKeys.Id
                        },
                        ExpressionAttributeValues = new Dictionary<string, DynamoDBEntry>
                        {
                            [":id"] = forward.Id
                        }
                    }
                });
            }
            catch (ConditionalCheckFailedException)
            {
                throw new InvalidOperationException("Cannot add a forward with a duplicate ID");
            }
        }

        public async Task<ForwardItem> GetForward(string id)
        {
            var doc = await _table.GetItemAsync(id, new GetItemOperationConfig
            {
                AttributesToGet = AllAttrs
            });

            if (doc != null)
            {
                return ForwardItem.FromDocument(doc);
            }
            throw new KeyNotFoundException("A forward with that ID was not found");
        }

        public async Task<IEnumerable<ForwardItem>> ListAllForwards()
        {
            var search = _table.Scan(new ScanOperationConfig());

            var forwards = new List<ForwardItem>();
            List<Document> docList;

            do
            {
                docList = await search.GetNextSetAsync();
                foreach (var doc in docList)
                {
                    forwards.Add(ForwardItem.FromDocument(doc));
                }
            } while (!search.IsDone);

            return forwards;
        }

        public async Task<string> ProcessForward(string id)
        {
            try
            {
                // Unfortunately, the document model API doesn't seem to
                // support the update expression (which is needed to update
                // the document without fetching it first), so we have to fall
                // back to the low-level API.
                var doc = await _dynamoDb.UpdateItemAsync(new UpdateItemRequest
                {
                    TableName = _configHelper.AppTable,
                    Key = new Dictionary<string, AttributeValue>
                    {
                        [FwdKeys.Id] = new AttributeValue { S = id }
                    },
                    UpdateExpression = "ADD #hitCounter :inc",
                    ConditionExpression = "#id = :id",
                    ExpressionAttributeNames = new Dictionary<string, string>
                    {
                        ["#id"] = FwdKeys.Id,
                        ["#hitCounter"] = FwdKeys.Hits
                    },
                    ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                    {
                        [":id"] = new AttributeValue { S = id },
                        [":inc"] = new AttributeValue { N = "1" }
                    },
                    ReturnValues = "ALL_NEW"
                });

                return doc.Attributes[FwdKeys.Dest].S;
            }
            catch (ConditionalCheckFailedException)
            {
                throw new KeyNotFoundException("A forward with that ID was not found");
            }
        }

        public async Task UpdateForward(string id, ForwardItemUpdate forward)
        {
            forward.Validate();

            var doc = forward.ToDocument(id);

            try
            {
                await _table.UpdateItemAsync(doc, new UpdateItemOperationConfig
                {
                    ConditionalExpression = new Expression
                    {
                        ExpressionStatement = "#id = :id",
                        ExpressionAttributeNames = new Dictionary<string, string>
                        {
                            ["#id"] = FwdKeys.Id
                        },
                        ExpressionAttributeValues = new Dictionary<string, DynamoDBEntry>
                        {
                            [":id"] = id
                        }

                    }
                });
            }
            catch (ConditionalCheckFailedException)
            {
                throw new KeyNotFoundException("A forward with that ID was not found");
            }
        }
    }
}
#pragma warning restore RECS0030 // Suggests using the class declaring a static function when calling it
