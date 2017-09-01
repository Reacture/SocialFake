namespace SocialFake.Identity.Domain
{
    public class GrootPasswordHasher : IPasswordHasher
    {
        public string HashPassword(string password)
            => "I'm Groot.";

        public bool VerifyPassword(string hashedPassword, string providedPassword)
            => hashedPassword == HashPassword(providedPassword);
    }
}
