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
        private static readonly RetryPolicy<bool> s_transientFalseRetryPolicy =
            RetryPolicy<bool>.LinearTransientDefault(
                maximumRetryCount: 5,
                increment: TimeSpan.FromMilliseconds(100));

        private readonly IMessageBus _messageBus;
        private readonly IdentityService _identityService;
        private readonly ReadModelFacade _readModelFacade;

        public UsersController(IMessageBus messageBus, IdentityService identityService, ReadModelFacade readModelFacade)
        {
            _messageBus = messageBus ?? throw new ArgumentNullException(nameof(messageBus));
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            _readModelFacade = readModelFacade ?? throw new ArgumentNullException(nameof(readModelFacade));
        }

        private Task<bool> CorrelationExists(Guid correlationId)
        {
            return s_transientFalseRetryPolicy.Run(() => _readModelFacade.CorrelationExists(correlationId));
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
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpPost]
        [Route("{id}/display-names")]
        [ResponseType(typeof(UserDto))]
        [SwaggerResponse(HttpStatusCode.Accepted)]
        public async Task<IHttpActionResult> PostDisplayNames(Guid id, ChangeDisplayNamesForm form)
        {
            if (form.FirstName == null ||
                form.MiddleName == null ||
                form.LastName == null)
            {
                return BadRequest();
            }

            var command = new ChangeDisplayNames
            {
                UserId = id,
                FirstName = form.FirstName,
                MiddleName = form.MiddleName,
                LastName = form.LastName
            };

            var envelope = new Envelope(command);

            await _messageBus.Send(envelope);

            if (await CorrelationExists(envelope.MessageId))
            {
                return Ok(await _readModelFacade.FindUser(id));
            }

            return StatusCode(HttpStatusCode.Accepted);
        }

        [HttpPost]
        [Route("{id}/bio")]
        [ResponseType(typeof(UserDto))]
        public async Task<IHttpActionResult> PostBio(Guid id, ChangeBioForm form)
        {
            if (form.Bio == null)
            {
                return BadRequest();
            }

            await _messageBus.Send(new Envelope(new ChangeBio
            {
                UserId = id,
                Bio = form.Bio
            }));

            UserDto user = await _readModelFacade.FindUser(id);
            user.Bio = form.Bio;
            return Ok(user);
        }
    }
}
