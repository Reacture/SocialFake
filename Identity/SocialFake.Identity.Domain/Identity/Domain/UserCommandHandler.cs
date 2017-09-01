using System;
using System.Threading;
using System.Threading.Tasks;
using Khala.EventSourcing;
using Khala.Messaging;
using SocialFake.Identity.Commands;

namespace SocialFake.Identity.Domain
{
    public class UserCommandHandler :
        InterfaceAwareHandler,
        IHandles<CreateUserWithPassword>
    {
        private readonly IPasswordHasher _passwordHasher;
        private readonly IEventSourcedRepository<User> _repository;

        public UserCommandHandler(
            IPasswordHasher passwordHasher,
            IEventSourcedRepository<User> repository)
        {
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public Task Handle(Envelope<CreateUserWithPassword> envelope, CancellationToken cancellationToken)
        {
            if (envelope == null)
            {
                throw new ArgumentNullException(nameof(envelope));
            }

            CreateUserWithPassword command = envelope.Message;
            string passwordHash = _passwordHasher.HashPassword(command.Password);
            var user = new User(command.UserId, command.Username, passwordHash);
            return _repository.Save(user);
        }
    }
}
