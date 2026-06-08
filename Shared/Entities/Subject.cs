using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Shared.Entities
{
    public class Subject
    {
        public int Id { get; set; }
        [Required]
        public required string Name { get; set; }
        public ICollection<Teacher> TeachedBy { get; set; } = new List<Teacher>();
        public ICollection<Student> StudiedBy { get; set; } = new List<Student>();

     
    }
}
