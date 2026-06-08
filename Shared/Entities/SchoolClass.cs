using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Shared.Entities
{
    public class SchoolClass
    {
        public int Id { get; set; }
        [Required]
        public required string Name { get; set; }
        [Required]
        public int Year { get; set; }
        public ICollection<Course> Courses { get; set; } = new List<Course>();

        public ICollection<Student> Students { get; set; } = new List<Student>();
    }
}
