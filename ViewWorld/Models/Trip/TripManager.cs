using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using RethinkDb.Driver.Net;
using ViewWorld.App_Start;
using ViewWorld.Core.Models;

namespace ViewWorld.Models.Trip
{
    public class TripManager : IDBOperational<Scenery>,IDBOperational<Location>
    {
        private ApplicationIdentityContext db = ApplicationIdentityContext.Create();

        public Cursor<Location> AddToDatabase(List<Location> ItemList, ApplicationIdentityContext db)
        {
            throw new NotImplementedException();
        }

        public Location AddToDatabase(Location Item, ApplicationIdentityContext db)
        {
            throw new NotImplementedException();
        }

        public Cursor<Scenery> AddToDatabase(List<Scenery> ItemList, ApplicationIdentityContext db)
        {
            throw new NotImplementedException();
        }

        public Scenery AddToDatabase(Scenery Item, ApplicationIdentityContext db)
        {
            throw new NotImplementedException();
        }

        public Task<Cursor<Location>> AddToDatabaseAsync(List<Location> ItemList, ApplicationIdentityContext db)
        {
            throw new NotImplementedException();
        }

        public Task<Location> AddToDatabaseAsync(Location Item, ApplicationIdentityContext db)
        {
            throw new NotImplementedException();
        }

        public Task<Cursor<Scenery>> AddToDatabaseAsync(List<Scenery> ItemList, ApplicationIdentityContext db)
        {
            throw new NotImplementedException();
        }

        public Task<Scenery> AddToDatabaseAsync(Scenery Item, ApplicationIdentityContext db)
        {
            throw new NotImplementedException();
        }

        public void addToDb(int id)
        {

        }

        public void RemoveFromDatabase(string ItemId, ApplicationIdentityContext db)
        {
            throw new NotImplementedException();
        }

        public void UpdateFromDatabase(string ItemId, ApplicationIdentityContext db)
        {
            throw new NotImplementedException();
        }
    }
}