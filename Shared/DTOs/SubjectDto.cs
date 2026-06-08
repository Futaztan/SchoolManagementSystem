using Shared.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Shared.DTOs
{
    public class SubjectDto
    {
      
        public int Id { get; set; }
        public string Name { get; set; }
        //public ICollection<TeacherDto> TeachedBy { get; set; } = new List<TeacherDto>();
        //public ICollection<StudentDto> StudiedBy { get; set; } = new List<StudentDto>();

        
    }
}
