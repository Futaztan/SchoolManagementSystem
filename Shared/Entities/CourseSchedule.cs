using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Shared.Entities
{
    public class CourseSchedule
    {
        public int Id { get; set; }
        [Required]
        public DayOfWeek DayOfWeek { get; set; }
        [Required]
        [Range(0,10)]
        public int LessonNumber { get; set; }

        public int? RoomId { get; set; }
        public Room? Room { get; set; }

        public int CourseId { get; set; }
        [Required]
        public required Course Course { get; set; }
    }
}
