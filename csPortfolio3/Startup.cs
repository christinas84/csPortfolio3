using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(csPortfolio3.Startup))]
namespace csPortfolio3
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
