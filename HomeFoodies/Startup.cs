using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(HomeFoodies.Startup))]
namespace HomeFoodies
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
