using ViewWorld.App_Start;

namespace ViewWorld.Core.Models
{
    public interface IDBOperational<T> where T :class
    {
        void RemoveFromDatabase(string ItemId, ApplicationIdentityContext db);
        void UpdateToDatabase(string ItemId, ApplicationIdentityContext db);
    }
}
