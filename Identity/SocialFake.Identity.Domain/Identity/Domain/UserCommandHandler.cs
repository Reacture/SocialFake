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
        IHandles<CreateUserWithPassword>,
        IHandles<ChangeDisplayNames>,
        IHandles<ChangeBio>
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
            return _repository.Save(user, envelope.MessageId, cancellationToken);
        }

        public async Task Handle(Envelope<ChangeDisplayNames> envelope, CancellationToken cancellationToken)
        {
            if (envelope == null)
            {
                throw new ArgumentNullException(nameof(envelope));
            }

            ChangeDisplayNames command = envelope.Message;
            User user = await GetUser(command.UserId);
            user.ChangeDisplayNames(command.FirstName, command.MiddleName, command.LastName);
            await _repository.Save(user, envelope.MessageId, cancellationToken);
        }

        public async Task Handle(Envelope<ChangeBio> envelope, CancellationToken cancellationToken)
        {
            if (envelope == null)
            {
                throw new ArgumentNullException(nameof(envelope));
            }

            ChangeBio command = envelope.Message;
            User user = await GetUser(command.UserId);
            user.ChangeBio(command.Bio);
            await _repository.Save(user, envelope.MessageId, cancellationToken);
        }

        private async Task<User> GetUser(Guid userId)
        {
            User user = await _repository.Find(userId);
            if (user == null)
            {
                throw new InvalidOperationException($"Could not find user with id '{userId}'.");
            }

            return user;
        }
    }
}
