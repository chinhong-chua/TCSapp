using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TCSapp.Startup))]
namespace TCSapp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
