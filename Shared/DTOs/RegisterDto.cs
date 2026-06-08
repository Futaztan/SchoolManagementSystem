using Shared.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.DTOs
{
    public class RegisterDto
    {
        public required string Name { get; set; }
        public required string Password { get; set; }
        public required string Email { get; set; }
        public required DateOnly BirthDate { get; set; }
        public required SchoolClassDto Class { get; set; }
    }
}
