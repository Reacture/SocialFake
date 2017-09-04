using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Khala.Messaging;
using SocialFake.Identity.Commands;

namespace SocialFake.Identity.Domain.Controllers
{
    [RoutePrefix("commands")]
    public class CommandsController : ApiController
    {
        private readonly IMessageHandler _handler;

        public CommandsController(IMessageHandler handler)
        {
            _handler = handler ?? throw new System.ArgumentNullException(nameof(handler));
        }

        [HttpPost]
        [Route(nameof(CreateUserWithPassword))]
        public async Task<IHttpActionResult> ExecuteCreateUserWithPassword(
            CreateUserWithPassword command)
        {
            await _handler.Handle(new Envelope(command));
            return StatusCode(HttpStatusCode.NoContent);
        }
    }
}
