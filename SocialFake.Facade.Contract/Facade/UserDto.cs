﻿using System;

namespace SocialFake.Facade
{
    public class UserDto
    {
        public Guid Id { get; set; }

        public string Username { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public string Bio { get; set; }
    }
}
