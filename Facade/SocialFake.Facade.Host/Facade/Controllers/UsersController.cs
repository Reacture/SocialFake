using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Khala.Messaging;
using Khala.TransientFaultHandling;
using SocialFake.Facade.ReadModel;
using SocialFake.Identity.Commands;
using SocialFake.Identity.Domain;
using Swashbuckle.Swagger.Annotations;

namespace SocialFake.Facade.Controllers
{
    [RoutePrefix("users")]
    public class UsersController : ApiController
    {
        private readonly IMessageBus _messageBus;
        private readonly IdentityService _identityService;
        private readonly ReadModelFacade _readModelFacade;

        public UsersController(IMessageBus messageBus, IdentityService identityService, ReadModelFacade readModelFacade)
        {
            _messageBus = messageBus ?? throw new ArgumentNullException(nameof(messageBus));
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

        [HttpPost]
        [Route("{id}/display-names")]
        [ResponseType(typeof(UserDto))]
        [SwaggerResponse(HttpStatusCode.Accepted)]
        public async Task<IHttpActionResult> PostDisplayNames(Guid id, ChangeDisplayNamesForm form)
        {
            if (string.IsNullOrWhiteSpace(form.FirstName) ||
                string.IsNullOrWhiteSpace(form.MiddleName) ||
                string.IsNullOrWhiteSpace(form.LastName))
            {
                return BadRequest();
            }

            var envelope = new Envelope(new ChangeDisplayNames
            {
                UserId = id,
                FirstName = form.FirstName,
                MiddleName = form.MiddleName,
                LastName = form.LastName
            });

            await _messageBus.Send(envelope);

            var retry = RetryPolicy<Correlation>.LinearTransientDefault(
                maximumRetryCount: 5,
                increment: TimeSpan.FromMilliseconds(200));

            Correlation correlation = await retry.Run(() => _readModelFacade.FindCorrelation(envelope.MessageId));

            return correlation == null
                ? StatusCode(HttpStatusCode.Accepted)
                : (IHttpActionResult)Ok(await _readModelFacade.FindUser(id));
        }
    }
}
