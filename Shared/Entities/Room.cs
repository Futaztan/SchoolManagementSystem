using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlTypes;
using System.Text;

namespace Shared.Entities
{
    public class Room
    {
        public int Id { get; set; }
        [Required]
        public required string Name { get; set; }

        public ICollection<CourseSchedule> CourseSchedules { get; set; } = new List<CourseSchedule>();
    }
}
