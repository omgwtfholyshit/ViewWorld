using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ViewWorld.Core.Models.OAuthModels;
using ViewWorld.Services.Authorization;

namespace ViewWorld.Services.ThirdParty
{
    public class WechatService : OauthService
    {
        private static readonly Lazy<WechatService> client = new Lazy<WechatService>(() => new WechatService());
        static WechatService()
        {
        }
        WechatService()
        {
        }
        public static WechatService Instance
        {
            get
            {
                return client.Value;
            }
        }
        public override string BuildAuthorizeUrl(string appId, string appKey, string state, string type)
        {
            throw new NotImplementedException();
        }

        public override Task<AuthorizationResult> GetTokenAsync(string appId, string appKey, HttpRequest callbackRequest)
        {
            throw new NotImplementedException();
        }
    }
}
