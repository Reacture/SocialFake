using System;
using Khala.EventSourcing;
using SocialFake.Identity.Events;

namespace SocialFake.Identity.Domain
{
    public class User : EventSourced
    {
        public User(Guid id, string username, string passwordHash)
            : base(id)
        {
            if (username == null)
            {
                throw new ArgumentNullException(nameof(username));
            }

            if (passwordHash == null)
            {
                throw new ArgumentNullException(nameof(passwordHash));
            }

            RaiseEvent(new UserCreated { Username = username });
            RaiseEvent(new PasswordHashChanged { PasswordHash = passwordHash });
        }

        private void Handle(UserCreated domainEvent)
        {
        }

        private void Handle(PasswordHashChanged domainEvent)
        {
        }
    }
}
