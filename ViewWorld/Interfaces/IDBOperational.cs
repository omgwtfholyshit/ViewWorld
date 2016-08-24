using RethinkDb.Driver.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViewWorld.App_Start;

namespace ViewWorld.Core.Models
{
    public interface IDBOperational<T> where T :class
    {
        T AddToDatabase(T Item,ApplicationIdentityContext db);
        Task<T> AddToDatabaseAsync(T Item, ApplicationIdentityContext db);
        Cursor<T> AddToDatabase(List<T> ItemList, ApplicationIdentityContext db);
        Task<Cursor<T>> AddToDatabaseAsync(List<T> ItemList, ApplicationIdentityContext db);
        void RemoveFromDatabase(string ItemId, ApplicationIdentityContext db);
        void UpdateFromDatabase(string ItemId, ApplicationIdentityContext db);
    }
}
