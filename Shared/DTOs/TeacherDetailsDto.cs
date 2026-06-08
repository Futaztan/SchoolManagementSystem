namespace Shared.DTOs;

public class TeacherDetailsDto : TeacherDto
{
    public string Email { get; set; }
    public string Password { get; set; }
    public DateOnly? BirthDate { get; set; }

    public ICollection<SubjectDto>? TeachedSubjects { get; set; } 
}