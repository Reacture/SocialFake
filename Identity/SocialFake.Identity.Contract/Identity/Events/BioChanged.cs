using Khala.EventSourcing;

namespace SocialFake.Identity.Events
{
    public class BioChanged : DomainEvent
    {
        public string Bio { get; set; }
    }
}
