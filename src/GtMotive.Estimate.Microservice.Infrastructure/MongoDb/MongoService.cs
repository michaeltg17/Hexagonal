using GtMotive.Estimate.Microservice.Infrastructure.MongoDb.Settings;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace GtMotive.Estimate.Microservice.Infrastructure.MongoDb
{
    public class MongoService
    {
        static MongoService()
        {
            BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
        }

        public MongoService(IOptions<MongoDbSettings> options)
        {
            MongoClient = new MongoClient(options.Value.ConnectionString);

            // Add call to RegisterBsonClasses() method.
        }

        public MongoClient MongoClient { get; }
    }
}
