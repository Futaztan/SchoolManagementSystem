using Shared.Entities;

namespace Client.Extensions;

public static class CourseExtensions
{
  
    public static CourseDto? GetCourseForSlot(this List<CourseDto>? Courses, DayOfWeek dayOfWeek, int lessonNumber)
    {
        if (Courses == null) return null;

        return Courses.FirstOrDefault(c => c.CourseSchedules.Any(s =>
            s.DayOfWeek == dayOfWeek && s.LessonNumber == lessonNumber));
    }
}