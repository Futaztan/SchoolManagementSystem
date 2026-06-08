using Shared.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Shared.DTOs
{
    public class SchoolClassDto
    {
     
        public int Id { get; set; }
        public string Name { get; set; }
       
        public int Year { get; set; }
        public ICollection<CourseDto> Courses { get; set; } = new List<CourseDto>();

        public ICollection<StudentDto> Students { get; set; } = new List<StudentDto>();
    }
}
