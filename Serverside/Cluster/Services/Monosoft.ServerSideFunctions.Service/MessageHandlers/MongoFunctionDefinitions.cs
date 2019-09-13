using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;
using System.Collections.Generic;

namespace Monosoft.ServerSideFunctions.Service.MessageHandlers
{
    class MongoFunctionDefinitions
    {
        [BsonElement("Id")]
        public string Id { get; set; }

        [BsonElement("Name")]
        public string Name { get; set; }

        [BsonElement("Function")]
        public string Function { get; set; }

        [BsonDictionaryOptions(DictionaryRepresentation.Document)]
        public Dictionary<string, string> BackUps { get; set; }
    }
}
