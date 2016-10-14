﻿using System;
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
        private ApplicationIdentityContext(IMongoClient client, IMongoDatabase db)
        {
            Client = client;
            DB = db;
        }

        public static ApplicationIdentityContext Create()
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase(DB_NAME);
            //var users = database.GetCollection<ApplicationUser>("users");
            //var roles = database.GetCollection<IdentityRole>("roles");
            return new ApplicationIdentityContext(client, database);
        }

        public static void initDatabase()
        {
            IMongoClient c;
            IMongoDatabase db;
            c = new MongoClient();
            db = c.GetDatabase("test");
            db.GetCollection<BsonDocument>("IdentityRoles");
            db.GetCollection<BsonDocument>("IdentityUsers");
            db.GetCollection<BsonDocument>("Trips");
            db.GetCollection<BsonDocument>("Sceneries");
            db.GetCollection<BsonDocument>("StartingPoints");
            db.GetCollection<BsonDocument>("Regions");
                                    
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
        public void Dispose()
        {

        }
    }
}