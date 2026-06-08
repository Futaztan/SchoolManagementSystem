using Shared.DTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Shared.Entities
{
    public class CourseDto
    {
        public int Id { get; set; }
   

    
        public SubjectDto Subject { get; set; }

 
        
        public int TeacherId { get; set; }
        public string TeacherName { get; set; }

   
        public int SchoolClassId { get; set; }
        public string SchoolClassName { get; set; }

        public ICollection<CourseScheduleDto>? CourseSchedules { get; set; } = new List<CourseScheduleDto>();
    }
}
