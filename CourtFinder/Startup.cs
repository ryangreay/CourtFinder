using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CourtFinder.Startup))]
namespace CourtFinder
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
