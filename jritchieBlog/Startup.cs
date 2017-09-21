using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(jritchieBlog.Startup))]
namespace jritchieBlog
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
