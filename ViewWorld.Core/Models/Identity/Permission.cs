using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewWorld.Core.Models.Identity
{
    public class Permission
    {
        public string Name { get; set; }
        public string ChineseName { get; set; }
        public string Description { get; set; }
        public Permission(string permissionName,string chineseName,string description)
        {
            this.Name = permissionName;
            this.ChineseName = chineseName;
            this.Description = description;
        }
        public Permission() { }
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (!(obj is Permission))
                return false;

            return this.Name == ((Permission)obj).Name && this.ChineseName == ((Permission)obj).ChineseName;
        }
        public override int GetHashCode()
        {
            return this.Name.GetHashCode() ^ this.ChineseName.GetHashCode() ^ this.Description.GetHashCode();
        }
    }

    public class PermissionStore
    {
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonRequired]
        public string Id { get; set; }
        public Permission Permission { get; set; }
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            if (!(obj is PermissionStore))
                return false;

            return Permission.Equals(((PermissionStore)obj).Permission);
        }
        public override int GetHashCode()
        {
            return this.Id.GetHashCode() ^ this.Permission.GetHashCode();
        }
    }
}
