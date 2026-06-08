using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Shared.Entities
{
    public class Course
    {
        public int Id { get; set; }
        public int SubjectId { get; set; }

      
        public Subject Subject { get; set; }

        public int? TeacherId { get; set; }
        
        public Teacher? Teacher { get; set; }

        public int SchoolClassId { get; set; }
       
        public  SchoolClass? SchoolClass { get; set; }
    
        public ICollection<CourseSchedule>? CourseSchedules { get; set; } = new List<CourseSchedule>();
    }
}
