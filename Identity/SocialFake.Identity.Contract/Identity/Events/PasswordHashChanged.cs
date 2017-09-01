using Khala.EventSourcing;

namespace SocialFake.Identity.Events
{
    public class PasswordHashChanged : DomainEvent
    {
        public string PasswordHash { get; set; }
    }
}
