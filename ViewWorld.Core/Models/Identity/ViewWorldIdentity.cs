﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace ViewWorld.Core.Models.Identity
{
    public class ViewWorldIdentity : IIdentity
    {
        public string UserName { get; set; }
        public string NickName { get; set; }
        public string Department { get; set; }
        public string UserPhone { get; set; }
        public ViewWorldIdentity(string username,string department,string phone,string nickname)
        {
            this.UserName = username;
            this.Department = department;
            this.UserPhone = phone;
            this.NickName = nickname;
        }
        public string AuthenticationType
        {
            get
            {
                return "Form";
            }
        }

        public bool IsAuthenticated
        {
            get
            {
                return !string.IsNullOrWhiteSpace(UserName);
            }
        }

        public string Name
        {
            get
            {
                return UserName;
            }
        }
    }
}
