using System;
using System.Collections.Generic;
using AspNet.Identity.RethinkDB;
using RethinkDb.Driver;
using RethinkDb.Driver.Ast;
using RethinkDb.Driver.Net;

namespace ViewWorld.App_Start
{
    public class ApplicationIdentityContext : IdentityContext, IDisposable
    {
        private const string DB_NAME = Core.Config.db_name;

        public ApplicationIdentityContext(IConnection conn, Db db)
            : base(conn, db)
        {
        }

        public static ApplicationIdentityContext Create()
        {
            // Load connection data from Web.config
            //IConnectionFactory connectionFactory = ConfigurationAssembler.CreateConnectionFactory("dbcluster");
            // Get the connection to the database server(s)
            IConnection databaseConnection = RethinkDb.Driver.RethinkDB.R.Connection().Hostname(RethinkDBConstants.DefaultHostname).Port(RethinkDBConstants.DefaultPort).Timeout(60).Connect();
            // Get an object to use the database
            //IDatabaseQuery DB = Query.Db(DB_NAME);
            Db DB = RethinkDB.R.Db(DB_NAME);
            return new ApplicationIdentityContext(databaseConnection, DB);
        }
        public static void initDatabase()
        {
            IConnection databaseConnection = RethinkDb.Driver.RethinkDB.R.Connection().Hostname(RethinkDBConstants.DefaultHostname).Port(RethinkDBConstants.DefaultPort).Timeout(60).Connect();
            // Get an object to use the database
            //IDatabaseQuery DB = Query.Db(DB_NAME);
            Db DB = RethinkDB.R.Db(DB_NAME);

            // Create DB if it does not exist
            //if (!databaseConnection.Run(Query.DbList()).Contains(DB_NAME))
            //	databaseConnection.Run(Query.DbCreate(DB_NAME));

            List<string> result = RethinkDB.R.DbList().RunResult<List<string>>(databaseConnection);
           
            if (!result.Contains(DB_NAME))
            {
                RethinkDB.R.DbCreate(DB_NAME).Run(databaseConnection);
            }

            result = RethinkDB.R.Db(DB_NAME).TableList().RunResult<List<string>>(databaseConnection);
            if (!result.Contains("IdentityRoles"))
                RethinkDB.R.Db(DB_NAME).TableCreate("IdentityRoles").Run(databaseConnection);
            if (!result.Contains("IdentityUsers"))
                RethinkDB.R.Db(DB_NAME).TableCreate("IdentityUsers").Run(databaseConnection);
            if(!result.Contains("Trips"))
                RethinkDB.R.Db(DB_NAME).TableCreate("Trips").Run(databaseConnection);
            if (!result.Contains("Sceneries"))
                RethinkDB.R.Db(DB_NAME).TableCreate("Sceneries").Run(databaseConnection);
            if (!result.Contains("StartingPoints"))
                RethinkDB.R.Db(DB_NAME).TableCreate("StartingPoints").Run(databaseConnection);
            if (!result.Contains("Regions"))
                RethinkDB.R.Db(DB_NAME).TableCreate("Regions").Run(databaseConnection);

            var dataCount = RethinkDB.R.Db(DB_NAME).Table("Sceneries").Count().Run<int>(databaseConnection);
            if (dataCount == 0)
                DataInitializer.Init();
        }
        public void Dispose()
        {

        }
    }
}