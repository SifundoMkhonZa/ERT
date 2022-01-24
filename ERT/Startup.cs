using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ERT.Startup))]
namespace ERT
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
