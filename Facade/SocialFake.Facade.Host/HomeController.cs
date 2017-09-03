using System;
using System.Web.Http;
using System.Web.Http.Description;

namespace SocialFake
{
    public class HomeController : ApiController
    {
        [HttpGet]
        [Route]
        [ApiExplorerSettings(IgnoreApi = true)]
        public IHttpActionResult Index()
        {
            return Redirect(new Uri("/swagger/ui/index", UriKind.Relative));
        }
    }
}