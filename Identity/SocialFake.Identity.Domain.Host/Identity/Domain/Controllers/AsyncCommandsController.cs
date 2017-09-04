using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Khala.Messaging;
using SocialFake.Identity.Commands;

namespace SocialFake.Identity.Domain.Controllers
{
    [RoutePrefix("async-commands")]
    public class AsyncCommandsController : ApiController
    {
        private readonly IMessageBus _messageBus;

        public AsyncCommandsController(IMessageBus messageBus)
        {
            _messageBus = messageBus ?? throw new ArgumentNullException(nameof(messageBus));
        }

        [HttpPost]
        [Route(nameof(CreateUserWithPassword))]
        public async Task<IHttpActionResult> SendCreateUserWithPasswordCommand(CreateUserWithPassword command)
        {
            await _messageBus.Send(new Envelope(command));
            return StatusCode(HttpStatusCode.Accepted);
        }

        [HttpPost]
        [Route(nameof(ChangeDisplayNames))]
        public async Task<IHttpActionResult> SendChangeDisplayNamesCommand(ChangeDisplayNames command)
        {
            await _messageBus.Send(new Envelope(command));
            return StatusCode(HttpStatusCode.Accepted);
        }
    }
}
