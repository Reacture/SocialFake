using System;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using SocialFake.Facade.ReadModel;

namespace SocialFake.Facade.Controllers
{
    [RoutePrefix("users")]
    public class UsersController : ApiController
    {
        private readonly ReadModelFacade _readModelFacade;

        public UsersController(ReadModelFacade readModelFacade)
        {
            _readModelFacade = readModelFacade ?? throw new ArgumentNullException(nameof(readModelFacade));
        }

        [HttpGet]
        [Route("{id}")]
        [ResponseType(typeof(UserDto))]
        public async Task<IHttpActionResult> Get(Guid id)
        {
            UserDto user = await _readModelFacade.FindUser(id);
            return user == null
                ? NotFound()
                : (IHttpActionResult)Ok(user);
        }
    }
}