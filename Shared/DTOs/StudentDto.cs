using Shared.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.DTOs
{
    public class StudentDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        
        //public SchoolClassDto? SchoolClass { get; set; }
        public int SchoolClassId { get; set; }
        public string SchoolClassName { get; set; }

        public ICollection<GradeDto> Grades { get; set; } = new List<GradeDto>();
    
    }
}
