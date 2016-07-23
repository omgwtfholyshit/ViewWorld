using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ViewWorld.Startup))]
namespace ViewWorld
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
