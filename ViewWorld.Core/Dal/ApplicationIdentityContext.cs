﻿using System;
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
        public ApplicationIdentityContext()
        {
            CreateDatabase();
        }
        void CreateDatabase()
        {
            Client = new MongoClient(connectionStr);
            DB = Client.GetDatabase(dbName);
            Users = DB.GetCollection<ApplicationUser>("users");
            Roles =DB.GetCollection<IdentityRole>("roles");
        }
        public static ApplicationIdentityContext Create()
        {
            return new ApplicationIdentityContext();
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
