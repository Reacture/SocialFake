using System;

namespace SocialFake.Identity.Functions
{
    public class VerifyPasswordArgs
    {
        public Guid UserId { get; set; }

        public string Password { get; set; }
    }
}
