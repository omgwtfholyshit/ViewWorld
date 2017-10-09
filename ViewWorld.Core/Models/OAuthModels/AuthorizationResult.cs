using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewWorld.Core.Models.OAuthModels
{
    /// <summary>
    /// 表示授权成功后平台返回的数据。
    /// </summary>
    public class AuthorizationResult
    {

        /// <summary>
        /// 访问令牌
        /// </summary>
        public string _token { get; set; }
        /// <summary>
        /// token的类型
        /// </summary>
        public string _tokenType { get; set; } = "Bearer";
        /// <summary>
        /// token到期时间
        /// </summary>
        public DateTime _expireAt { get; set; }
        /// <summary>
        /// 刷新令牌
        /// </summary>
        public string _refreshToken { get; set; }
        /// <summary>
        /// 刷新令牌的到期时间
        /// </summary>
        public DateTime _refreshExpireAt { get; set; }
        /// <summary>
        /// 用户在Etp的用户名
        /// </summary>
        public string _userName { get; set; }
        /// <summary>
        /// 用户在Etp的唯一标识
        /// </summary>
        public string _openId { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("访问令牌是:{0}", _token);
            sb.AppendFormat("token的类型是:{0}", _tokenType);
            sb.AppendFormat("token到期时间是:{0}", _expireAt);
            sb.AppendFormat("刷新令牌是:{0}", _refreshToken);
            sb.AppendFormat("刷新令牌的到期时间是:{0}", _refreshExpireAt);
            sb.AppendFormat("用户在Etp的用户名:{0}", _userName);
            sb.AppendFormat("唯一标识是:{0}", _openId);
            return sb.ToString();
        }
    }//end AuthorizationResult
}
