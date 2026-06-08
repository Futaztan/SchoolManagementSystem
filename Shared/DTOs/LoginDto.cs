using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.DTOs
{
    public class LoginDto
    {
        public required string Name { get; set; }
        public required string Password { get; set; }
    }
}
