using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ViewWorld.Core.Models.OAuthModels;

namespace ViewWorld.Services.Authorization
{
    public interface IOauthService
    {
        /// <summary>
        /// 生成跳转URL
        /// </summary>
        /// <param name="appId">应用ID</param>
        /// <param name="appKey">应用秘钥</param>
        /// <param name="state"></param>
        /// <param name="type">授权页面的类型，如淘宝将授权页面分为web、tmall和wap三种类型</param>
        /// <returns></returns>
        string BuildAuthorizeUrl(string appId, string appKey, string state, string type);
        /// <summary>
        /// 接受回调请求后向平台换取Token。
        /// </summary>
        /// <param name="callbackRequest">回调请求。</param>
        Task<AuthorizationResult> GetToken(string appId, string appKey, HttpRequest callbackRequest);

        /// <summary>
        /// 异步向平台换取Token。
        /// </summary>
        /// <param name="callbackRequest">回调请求。</param>
        Task<AuthorizationResult> GetTokenAsync(string appId, string appKey, HttpRequest callbackRequest);

    }
}
