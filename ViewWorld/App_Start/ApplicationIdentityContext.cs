using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using ViewWorld.Models;
using AspNet.Identity.MongoDB;

namespace ViewWorld.App_Start
{
    public class ApplicationIdentityContext : IDisposable
    {        
        private const string DB_NAME = Core.Config.db_name;
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
            IMongoClient client = new MongoClient("mongodb://localhost:27017");
            IMongoDatabase database = client.GetDatabase(DB_NAME);
            var users = database.GetCollection<ApplicationUser>("users");
            var roles = database.GetCollection<IdentityRole>("roles");
            return new ApplicationIdentityContext(client, database,users,roles);
        }

        public static void initDatabase()
        {
            IMongoClient c;
            IMongoDatabase db;
            c = new MongoClient();
            db = c.GetDatabase(DB_NAME);
                        
            if (!CollectionExists(db, "IdentityRoles"))
            {
                db.CreateCollection("IdentityRoles");
            }
            if (!CollectionExists(db, "IdentityUsers"))
            {
                db.CreateCollection("IdentityUsers");
            }
            if (!CollectionExists(db, "Trips"))
            {
                db.CreateCollection("Trips");
            }
            if (!CollectionExists(db, "Sceneries"))
            {
                db.CreateCollection("Sceneries");
            }
            if (!CollectionExists(db, "StartingPoints"))
            {
                db.CreateCollection("StartingPoints");
            }
            if (!CollectionExists(db, "Regions"))
            {
                db.CreateCollection("Regions");
            }

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
            return DB.GetCollection<TEntity>(typeof(TEntity).Name + "s");
        }
        public void Dispose()
        {

        }
    }
}