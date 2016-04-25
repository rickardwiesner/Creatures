using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ClashOfTheCharacters.Startup))]
namespace ClashOfTheCharacters
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
