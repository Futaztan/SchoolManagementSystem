namespace Shared.DTOs;

public class StudentDetailsDto : StudentDto
{
    public string? Password { get; set; }
    public string? Email { get; set; }
    public DateOnly? BirthDate { get; set; }
}