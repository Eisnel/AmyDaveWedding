using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AmyDaveWedding.Startup))]
namespace AmyDaveWedding
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
