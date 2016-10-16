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

            //IConnection databaseConnection = RethinkDb.Driver.RethinkDB.R.Connection().Hostname(RethinkDBConstants.DefaultHostname).Port(RethinkDBConstants.DefaultPort).Timeout(60).Connect();
            //// Get an object to use the database
            ////IDatabaseQuery DB = Query.Db(DB_NAME);
            //Db DB = RethinkDB.R.Db(DB_NAME);

            //// Create DB if it does not exist
            ////if (!databaseConnection.Run(Query.DbList()).Contains(DB_NAME))
            ////	databaseConnection.Run(Query.DbCreate(DB_NAME));

            //List<string> result = RethinkDB.R.DbList().RunResult<List<string>>(databaseConnection);

            //if (!result.Contains(DB_NAME))
            //{
            //    RethinkDB.R.DbCreate(DB_NAME).Run(databaseConnection);
            //}

            //result = RethinkDB.R.Db(DB_NAME).TableList().RunResult<List<string>>(databaseConnection);
            //if (!result.Contains("IdentityRoles"))
            //    RethinkDB.R.Db(DB_NAME).TableCreate("IdentityRoles").Run(databaseConnection);
            //if (!result.Contains("IdentityUsers"))
            //    RethinkDB.R.Db(DB_NAME).TableCreate("IdentityUsers").Run(databaseConnection);
            //if (!result.Contains("Trips"))
            //    RethinkDB.R.Db(DB_NAME).TableCreate("Trips").Run(databaseConnection);
            //if (!result.Contains("Sceneries"))
            //    RethinkDB.R.Db(DB_NAME).TableCreate("Sceneries").Run(databaseConnection);
            //if (!result.Contains("StartingPoints"))
            //    RethinkDB.R.Db(DB_NAME).TableCreate("StartingPoints").Run(databaseConnection);
            //if (!result.Contains("Regions"))
            //    RethinkDB.R.Db(DB_NAME).TableCreate("Regions").Run(databaseConnection);

            //var dataCount = RethinkDB.R.Db(DB_NAME).Table("Sceneries").Count().Run<int>(databaseConnection);
            //if (dataCount == 0)
            //    DataInitializer.Init();
        }
        private static bool CollectionExists(IMongoDatabase db, string collectionName)
        {
            var filter = new BsonDocument("name", collectionName);
            //filter by collection name
            var collections = db.ListCollections(new ListCollectionsOptions { Filter = filter });
            //check for existence
            return collections.ToList().Any();
        }
        public void Dispose()
        {

        }
    }
}