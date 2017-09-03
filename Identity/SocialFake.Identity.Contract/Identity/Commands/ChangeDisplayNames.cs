using System;
using Khala.Messaging;
using Newtonsoft.Json;

namespace SocialFake.Identity.Commands
{
    public class ChangeDisplayNames : IPartitioned
    {
        public Guid UserId { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        [JsonIgnore]
        public string PartitionKey => UserId.ToString();
    }
}
