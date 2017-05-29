using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(_1361071.Startup))]
namespace _1361071
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
