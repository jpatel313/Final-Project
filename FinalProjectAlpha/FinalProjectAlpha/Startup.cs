using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FinalProjectAlpha.Startup))]
namespace FinalProjectAlpha
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
