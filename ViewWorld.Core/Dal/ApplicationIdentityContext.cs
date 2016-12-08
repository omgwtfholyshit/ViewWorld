using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using ViewWorld.Models;
using AspNet.Identity.MongoDB;
using ViewWorld.Core;

namespace ViewWorld.Core.Dal
{
    public class ApplicationIdentityContext : IDisposable
    {
        static string dbName = "ViewWorld";
        static string connectionStr = string.Format("mongodb://{0}:{1}", Config.DB_HostIP, Config.DB_Port);
        public IMongoClient Client { get; set; }
        public IMongoDatabase DB { get; set; }
        public IMongoCollection<IdentityRole> Roles { get; set; }
        public IMongoCollection<ApplicationUser> Users { get; set; }
        private ApplicationIdentityContext(IMongoClient client, IMongoDatabase db, IMongoCollection<ApplicationUser> users, IMongoCollection<IdentityRole> roles)
        {
            Client = client;
            DB = db;
            Users = users;
            Roles = roles;
        }

        public static ApplicationIdentityContext Create()
        {
            IMongoClient client = new MongoClient(connectionStr);
            IMongoDatabase database = client.GetDatabase(dbName);
            var users = database.GetCollection<ApplicationUser>("users");
            var roles = database.GetCollection<IdentityRole>("roles");
            return new ApplicationIdentityContext(client, database, users, roles);
        }
        private static bool CollectionExists(IMongoDatabase db, string collectionName)
        {
            var filter = new BsonDocument("name", collectionName);
            //filter by collection name
            var collections = db.ListCollections(new ListCollectionsOptions { Filter = filter });
            //check for existence
            return collections.ToList().Any();
        }
        public IMongoCollection<TEntity> GetCollection<TEntity>()
        {
            if (typeof(TEntity).Name == "ApplicationUser")
            {
                return DB.GetCollection<TEntity>("users");
            }
            return DB.GetCollection<TEntity>(typeof(TEntity).Name + "s");
        }
        public void Dispose()
        {

        }
    }
}
