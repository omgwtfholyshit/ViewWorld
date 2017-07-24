using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ViewWorld.Core.Models.BusinessModels;

namespace ViewWorld.ViewModels
{
    public class OrderOrderDetailViewModel
    {
        public BusinessOrder Order { get; set; }
        public string Role { get; set; }
        public bool validUser { get; set; }

    }
}