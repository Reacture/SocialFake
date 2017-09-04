using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using SocialFake.Facade.ReadModel;
using SocialFake.Identity.Commands;
using SocialFake.Identity.Domain;
using Swashbuckle.Swagger.Annotations;

namespace SocialFake.Facade.Controllers
{
    [RoutePrefix("users")]
    public class UsersController : ApiController
    {
        private readonly IdentityService _identityService;
        private readonly ReadModelFacade _readModelFacade;

        public UsersController(IdentityService identityService, ReadModelFacade readModelFacade)
        {
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            _readModelFacade = readModelFacade ?? throw new ArgumentNullException(nameof(readModelFacade));
        }

        [HttpPost]
        [Route]
        [ResponseType(typeof(UserDto))]
        public async Task<IHttpActionResult> Post(SignUpForm form)
        {
            var command = new CreateUserWithPassword
            {
                UserId = Guid.NewGuid(),
                Username = form.Username,
                Password = form.Password
            };

            await _identityService.ExecuteCommand(command);

            return Ok(new UserDto
            {
                Id = command.UserId,
                Username = command.Username,
                FirstName = string.Empty,
                MiddleName = string.Empty,
                LastName = string.Empty
            });
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