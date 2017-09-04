using System;
using Khala.Messaging;
using Newtonsoft.Json;

namespace SocialFake.Identity.Commands
{
    public class ChangeBio : IPartitioned
    {
        public Guid UserId { get; set; }

        public string Bio { get; set; }

        [JsonIgnore]
        public string PartitionKey => UserId.ToString();
    }
}
