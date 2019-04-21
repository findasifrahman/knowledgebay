using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(KioskNavy.Startup))]
namespace KioskNavy
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
