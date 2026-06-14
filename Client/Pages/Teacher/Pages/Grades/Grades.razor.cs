using System.Globalization;
using System.Net.Http.Json;
using System.Security.Claims;
using Shared.DTOs;
using Shared.Entities;

namespace Client.Pages.Teacher.Pages.Grades;

public partial class Grades
{
    private List<SchoolClassDto> schoolClasses = new List<SchoolClassDto>();
    private Dictionary<StudentDto, string> newGradeDic = new();
    private SelectionModel selectionModel = new();
    private string? teacherName;
    private string? teacherId;
    private string? submittedData;
    private bool IsSubmited = false;
    private SchoolClassDto? SelectedClass => schoolClasses.SingleOrDefault(c => c.Id == selectionModel.SelectedClassId);

    private CourseDto? SelectedCourse => schoolClasses.SelectMany(s => s.Courses)
        .SingleOrDefault(c => c.Id == selectionModel.SelectedCourseId);

    private List<StudentDto>? students;

    public class SelectionModel
    {
        public int? SelectedClassId { get; set; } = null;
        public int? SelectedCourseId { get; set; } = null;
    }

    private async Task HandleSubmit()
    {
        var response = await Http.GetFromJsonAsync<List<StudentDto>>($"api/grades/?cId={SelectedClass.Id}");
        if (response != null)
        {
            students = response;
            foreach (var student in students)
            {
                student.Grades = student.Grades.Where(g => g.CourseId == selectionModel.SelectedCourseId).ToList();
            }
        }
    }

    private async Task OnSaveNewGrades()
    {
        var newGrades = newGradeDic.Where(kvp => kvp.Value != "-").ToList();

        if (!newGrades.Any())
        {
            return;
        }

        List<GradeDto> newGradesList = new List<GradeDto>();

        foreach (var kvp in newGrades)
        {
            GradeDto dto = new GradeDto
            {
                CourseId = (int)selectionModel.SelectedCourseId!, Date = DateOnly.FromDateTime(DateTime.Now),
                StudentId = kvp.Key.Id, Value = Int32.Parse(kvp.Value)
            };
            newGradesList.Add(dto);
        }

        var response = await Http.PostAsJsonAsync("api/grades", newGradesList);
        if (response.IsSuccessStatusCode)
        {
            foreach (var grade in newGradesList)
            {
                students.Single(s => s.Id == grade.StudentId).Grades.Add(grade);
            }

            newGradeDic.Clear();
            StateHasChanged();
        }
    }


    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        teacherName = user.Identity.Name;
        teacherId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var response = await Http.GetFromJsonAsync<List<SchoolClassDto>>("api/classes");
        if (response != null) schoolClasses = response;
    }
}