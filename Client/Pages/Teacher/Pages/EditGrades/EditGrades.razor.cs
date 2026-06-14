using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Shared.DTOs;

namespace Client.Pages.Teacher.Pages.EditGrades;

public partial class EditGrades : ComponentBase
{
    public class SelectionModel
    {
        public int? SelectedClassId { get; set; } = null;
        public int? SelectedStudentId { get; set; } = null;
        public int? SelectedCourseId { get; set; } = null;
    }

    public class GradesListModel
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public int Grade { get; set; }
        public DateOnly Date { get; set; }
    }

    private List<GradesListModel> gradesListModel = new();
   



    private SelectionModel selectionModel = new();
    private List<SchoolClassDto>? schoolClasses = null;
    private StudentDto? student = null;

    private async Task GetStudentGrades()
    {
        var response =
            await Http.GetFromJsonAsync<List<StudentDto>>($"api/grades/?sId={selectionModel.SelectedStudentId}");
        if (response != null)
        {
            student = response[0];
            student.Grades = student.Grades.Where(g => g.CourseId == selectionModel.SelectedCourseId).ToList();
            foreach (var grade in student.Grades)
            {
                gradesListModel.Add(new GradesListModel
                    { Id = grade.Id, Subject = grade.CourseName, Grade = grade.Value, Date = grade.Date });
            }


        }
    }

    protected override async Task OnInitializedAsync()
    {
        var response = await Http.GetFromJsonAsync<List<SchoolClassDto>>("api/classes");
        if (response != null) schoolClasses = response;
    }




}