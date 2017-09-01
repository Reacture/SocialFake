using Owin;
using SocialFake.Identity.Domain;

[assembly: Microsoft.Owin.OwinStartup(typeof(Startup))]

namespace SocialFake.Identity.Domain
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
        }
    }
}
