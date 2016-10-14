using System;
using System.Runtime.Serialization;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNet.Identity.MongoDB;
using Microsoft.AspNet.Identity;

namespace ViewWorld.Models
{
    [DataContract]
    public class ApplicationUser : IdentityUser
    {
        [DataMember]
        public string NickName { get; set; }
        [DataMember]
        public string Avatar { get; set; }

        [DataMember]
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
        public DateTime DOB { get; set; }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }
}