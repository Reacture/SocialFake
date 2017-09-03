using Khala.EventSourcing;

namespace SocialFake.Identity.Events
{
    public class DisplayNamesChanged : DomainEvent
    {
        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }
    }
}
