using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Shared.Entities
{
    public class Student
    {
        public int Id { get; set; }
        [Required]
        public required string Name { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        public required DateOnly BirthDate { get; set; }

    
        public SchoolClass? SchoolClass { get; set; }
        public int SchoolClassId { get; set; }

        public ICollection<Subject> Subjects { get; set; } = new List<Subject>();
        public ICollection<Grade> Grades { get; set; } = new List<Grade>();
        public string? UserId { get; set; }
        public IdentityUser? User { get; set; }

    }
}
