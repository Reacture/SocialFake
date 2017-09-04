using System;
using System.Collections.Generic;
using Khala.EventSourcing;
using SocialFake.Identity.Events;

namespace SocialFake.Identity.Domain
{
    public class User : EventSourced
    {
        public const int MaximumDisplayNameLength = 100;
        public const int MaximumBioLength = 1000;

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

        public void ChangeDisplayNames(string firstName, string middleName, string lastName)
        {
            if (firstName == null)
            {
                throw new ArgumentNullException(nameof(firstName));
            }

            if (firstName.Length > MaximumDisplayNameLength)
            {
                throw new ArgumentException(
                    $"Value cannot be longer than {MaximumDisplayNameLength}.",
                    nameof(firstName));
            }

            if (middleName == null)
            {
                throw new ArgumentNullException(nameof(middleName));
            }

            if (middleName.Length > MaximumDisplayNameLength)
            {
                throw new ArgumentException(
                    $"Value cannot be longer than {MaximumDisplayNameLength}.",
                    nameof(middleName));
            }

            if (lastName == null)
            {
                throw new ArgumentNullException(nameof(lastName));
            }

            if (lastName.Length > MaximumDisplayNameLength)
            {
                throw new ArgumentException(
                    $"Value cannot be longer than {MaximumDisplayNameLength}.",
                    nameof(lastName));
            }

            RaiseEvent(new DisplayNamesChanged
            {
                FirstName = firstName,
                MiddleName = middleName,
                LastName = lastName
            });
        }

        public void ChangeBio(string bio)
        {
            if (bio == null)
            {
                throw new ArgumentNullException(nameof(bio));
            }

            if (bio.Length > MaximumBioLength)
            {
                throw new ArgumentException(
                    $"Value cannot be longer than {MaximumBioLength}",
                    nameof(bio));
            }

            RaiseEvent(new BioChanged { Bio = bio });
        }

        private void Handle(UserCreated domainEvent)
        {
        }

        private void Handle(PasswordHashChanged domainEvent)
        {
        }

        private void Handle(DisplayNamesChanged domainEvent)
        {
        }

        private void Handle(BioChanged domainEvent)
        {
        }
    }
}
