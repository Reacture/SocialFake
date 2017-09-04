using Newtonsoft.Json;

namespace SocialFake.Facade.ReadModel
{
    public static class UserDtoAssembler
    {
        public static UserDto AssembleDto(this User entity)
        {
            if (entity == null)
            {
                throw new System.ArgumentNullException(nameof(entity));
            }

            var displayNames = JsonConvert.DeserializeObject<DisplayNames>(entity.DisplayNamesJson);

            return new UserDto
            {
                Id = entity.Id,
                Username = entity.Username,
                FirstName = displayNames.FirstName,
                MiddleName = displayNames.MiddleName,
                LastName = displayNames.LastName,
                Bio = entity.Bio
            };
        }
    }
}
