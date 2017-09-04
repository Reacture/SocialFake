using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using Khala.EventSourcing;
using SocialFake.Identity.Functions;
using Swashbuckle.Swagger.Annotations;

namespace SocialFake.Identity.Domain.Controllers
{
    [RoutePrefix("functions")]
    public class FunctionsController : ApiController
    {
        private readonly IEventSourcedRepository<User> _repository;
        private readonly IPasswordHasher _passwordHasher;

        public FunctionsController(
            IEventSourcedRepository<User> repository,
            IPasswordHasher passwordHasher)
        {
            _repository = repository ?? throw new System.ArgumentNullException(nameof(repository));
            _passwordHasher = passwordHasher ?? throw new System.ArgumentNullException(nameof(passwordHasher));
        }

        [Route("verify-password")]
        [SwaggerResponseRemoveDefaults]
        [SwaggerResponse(HttpStatusCode.NoContent)]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        public async Task<IHttpActionResult> VerifyPassword(VerifyPasswordArgs args)
        {
            User user = await _repository.Find(args.UserId);
            if (user == null)
            {
                return BadRequest();
            }

            if (_passwordHasher.VerifyPassword(user.PasswordHash, args.Password))
            {
                return Ok();
            }

            return BadRequest();
        }
    }
}