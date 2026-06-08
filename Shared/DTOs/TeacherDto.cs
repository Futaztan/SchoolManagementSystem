using Microsoft.AspNetCore.Identity;
using Shared.Entities;
using System.ComponentModel.DataAnnotations;



namespace Shared.DTOs
{
    public class TeacherDto
    {
       
        public string Name { get; set; }
        public int Id { get; set; }

        
        public ICollection<CourseDto> Courses { get; set; }
     

    }
}
