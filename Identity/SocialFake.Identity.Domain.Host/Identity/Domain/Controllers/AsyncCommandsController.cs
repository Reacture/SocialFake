using System;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Khala.Messaging;
using SocialFake.Identity.Commands;
using Swashbuckle.Swagger.Annotations;

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
        [Route(nameof(ChangeDisplayNames))]
        [SwaggerResponse(HttpStatusCode.Accepted)]
        [ResponseType(typeof(CommandMessageReference))]
        public async Task<IHttpActionResult> SendChangeDisplayNamesCommand(ChangeDisplayNames command)
        {
            var envelope = new Envelope(command);
            await _messageBus.Send(envelope);
            return Content(HttpStatusCode.Accepted, new CommandMessageReference
            {
                MessageId = envelope.MessageId
            });
        }
    }
}
