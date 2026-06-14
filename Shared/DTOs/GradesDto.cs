using Shared.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.DTOs
{
    public class GradeDto
    {
        public int Id { get; set; }
      
        public int Value { get; set; }
     
        public int CourseId { get; set; }
        public string? CourseName { get; set; }
        public int StudentId { get; set; }
       
        public DateOnly Date { get; set; }
    }
}
