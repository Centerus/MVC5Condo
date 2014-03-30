using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MVC5Condo.Startup))]
namespace MVC5Condo
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
