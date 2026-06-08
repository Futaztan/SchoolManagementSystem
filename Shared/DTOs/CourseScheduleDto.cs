using Shared.DTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Shared.Entities
{
    public class CourseScheduleDto
    {
        public int Id { get; set; }
      
        public DayOfWeek DayOfWeek { get; set; } 

        [Range(0,10)]
        public int LessonNumber { get; set; }

        
        public RoomDto Room { get; set; }

    
        //public required CourseDto Course { get; set; }
        public int CourseId { get; set; }
        public string CourseName { get; set; }
    }
}
