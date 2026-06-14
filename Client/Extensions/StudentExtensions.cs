using Client.Constants;
using Shared.DTOs;

namespace Client.Extensions;

public static class StudentExtensions
{
    public static List<GradeDto> GetMonthlyGradeData(this StudentDto student, int monthNumber)
    {

        if (student?.Grades == null)
        {
            return new List<GradeDto>();
        }


        return student.Grades
            .Where(g => g.Date.Month == monthNumber)
            .ToList();
    }
    public static  List<GradeDto> GetMonthlyDataByCourse(this StudentDto student, int monthNumber, string course)
    {
        if (student?.Grades == null)
        {
            return new List<GradeDto>();
        }
        var g = student.Grades.Where(g => g.CourseName == course && g.Date.Month == monthNumber).ToList();
        return g;
    }
    public static double GetGradeSumAverage(this StudentDto student)
    {
        int sum = 0;
        int count = 0;
        foreach (var month in AppConstants.Months)
        {
            var gradesList = student.GetMonthlyGradeData(month);
            foreach (var grade in gradesList)
            {
                sum += grade.Value;
                count++;
            }
        }

        if (count == 0) return 0;
        return Math.Round((double)sum / count, 2);
    }
    
    public static double GetGradeAverageByCourse(this StudentDto student, string course)
    {
        var gradeByCourse = student.Grades.Where(g => g.CourseName == course).ToList();
        if (gradeByCourse.Count == 0) return 0;
        return Math.Round(gradeByCourse.Average(g => g.Value), 2);
    }
}