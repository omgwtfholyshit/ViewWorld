using System;
using System.Runtime.Serialization;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNet.Identity.MongoDB;
using Microsoft.AspNet.Identity;
using ViewWorld.Core.Enum;
using MongoDB.Bson.Serialization.Attributes;

namespace ViewWorld.Core.Models.Identity
{
    [DataContract]
    public class ApplicationUser : IdentityUser
    {
        [DataMember]
        public string NickName { get; set; }
        [DataMember]
        public string Avatar { get; set; }

        [DataMember]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime RegisteredAt { get; set; }
        [DataMember]
        public string WechatOpenId { get; set; }
        [DataMember]
        public string WechatUnionId { get; set; }
        [DataMember]
        public string Country { get; set; }
        [DataMember]
        public string Province { get; set; }
        [DataMember]
        public string City { get; set; }
        [DataMember]
        public string District { get; set; }
        [DataMember]
        public string Address { get; set; }
        [DataMember]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime DOB { get; set; }
        [DataMember]
        public SexType Sex { get; set; }
        [DataMember]
        public int Points { get; set; }
        [DataMember]
        public string Department { get; set; }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }
}