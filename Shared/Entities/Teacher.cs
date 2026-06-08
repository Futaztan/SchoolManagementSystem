using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Shared.Entities
{
    public class Teacher
    {
        public int Id { get; set; }
        [Required]
        public required string Name { get; set; }
        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        public ICollection<Subject>? TeachedSubjects { get; set; } = new List<Subject>();
        public string? UserId { get; set; }
        public IdentityUser? User { get; set; }


    }
}
