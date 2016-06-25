using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CryptoMVC.Startup))]
namespace CryptoMVC
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
