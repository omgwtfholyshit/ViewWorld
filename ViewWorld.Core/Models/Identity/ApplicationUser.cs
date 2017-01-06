using System;
using System.Runtime.Serialization;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNet.Identity.MongoDB;
using Microsoft.AspNet.Identity;
using ViewWorld.Core.Enum;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

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
        [NotMapped]
        List<Permission> _Permissions;
        [DataMember]
        public List<Permission> Permissions {
            get {
                return _Permissions ?? new List<Permission>();
            }
            set {
                _Permissions = value;
            }
        }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);

            // Add custom user claims
            List<Claim> claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.Name, NickName));
            claims.Add(new Claim(ClaimTypes.Email, Email ?? ""));
            claims.Add(new Claim(ClaimTypes.MobilePhone, PhoneNumber ?? ""));
            claims.Add(new Claim(ClaimTypes.DateOfBirth, DOB.ToString()));
            userIdentity.AddClaims(claims);

            //Update HttpContext.Current.User
            ViewWorldPrincipal principal = new ViewWorldPrincipal(Roles, Permissions, userIdentity);
            if(System.Web.HttpContext.Current !=null)
                System.Web.HttpContext.Current.User = principal;
            return userIdentity;
        }
        
    }
}