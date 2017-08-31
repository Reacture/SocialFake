using System.Collections.Generic;
using Khala.EventSourcing;
using Khala.EventSourcing.Sql;
using Newtonsoft.Json;

namespace SocialFake.Identity.Events
{
    public class UserCreated : DomainEvent, IUniqueIndexedDomainEvent
    {
        public string Username { get; set; }

        [JsonIgnore]
        public IReadOnlyDictionary<string, string> UniqueIndexedProperties
            => new Dictionary<string, string>
            {
                [nameof(Username)] = Username
            };
    }
}
