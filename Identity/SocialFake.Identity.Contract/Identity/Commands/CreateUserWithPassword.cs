using System;
using Khala.Messaging;

namespace SocialFake.Identity.Commands
{
    public class CreateUserWithPassword : IPartitioned
    {
        public Guid UserId { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string PartitionKey => UserId.ToString();
    }
}
