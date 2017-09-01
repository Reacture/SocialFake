using System;
using System.Collections.Generic;
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

        private User(Guid id, IEnumerable<IDomainEvent> pastEvents)
            : base(id)
        {
            HandlePastEvents(pastEvents);
        }

        public static User Factory(Guid id, IEnumerable<IDomainEvent> pastEvents)
        {
            return new User(id, pastEvents);
        }

        private void Handle(UserCreated domainEvent)
        {
        }

        private void Handle(PasswordHashChanged domainEvent)
        {
        }
    }
}
