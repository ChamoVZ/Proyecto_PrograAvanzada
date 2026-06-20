using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AP.MVC.Startup))]
namespace AP.MVC
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
