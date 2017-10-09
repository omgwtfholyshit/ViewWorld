using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ViewWorld.Core.Models.OAuthModels;

namespace ViewWorld.Services.Authorization
{
    public abstract class OauthService : IOauthService
    {
        public abstract string BuildAuthorizeUrl(string appId, string appKey, string state, string type);

        public virtual async Task<AuthorizationResult> GetToken(string appId, string appKey, HttpRequest callbackRequest)
        {
            try
            {
                return await GetTokenAsync(appId, appKey, callbackRequest);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public abstract Task<AuthorizationResult> GetTokenAsync(string appId, string appKey, HttpRequest callbackRequest);
    }
}
