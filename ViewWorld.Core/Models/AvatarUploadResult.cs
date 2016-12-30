using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewWorld.Core.Models
{
    public struct AvatarUploadResult
    {
        /// <summary>
        /// 表示图片是否已上传成功。
        /// </summary>
        public bool success;
        /// <summary>
        ///
        /// </summary>
        //public string userid;
        /// <summary>
        ///
        /// </summary>
        //public string username;
        /// <summary>
        /// 自定义的附加消息。
        /// </summary>
        public string msg;
        /// <summary>
        /// 表示原始图片的保存地址。
        /// </summary>2
        //public string sourceUrl;
        /// <summary>
        /// 表示所有头像图片的保存地址，该变量为一个数组。
        /// </summary>
        public ArrayList avatarUrls;
    }
    public struct DropdownDataStruct
    { 
        // grouping for all dropdown values
        public string name { get; set; }
       
        // actual dropdown value
        public string values { get; set; }

        // grouping for api results
        public string results { get; set; }
    }
}
