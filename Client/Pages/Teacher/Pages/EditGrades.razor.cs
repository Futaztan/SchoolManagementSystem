using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Shared.DTOs;

namespace Client.Pages.Teacher.Pages;

public partial class EditGrades : ComponentBase
{
    private class SelectionModel
    {
        public int? SelectedClassId { get; set; } = null;
        public int? SelectedStudentId { get; set; } = null;
        public int? SelectedCourseId { get; set; } = null;
    }

    private class GradesListModel
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public int Grade { get; set; }
        public DateOnly Date { get; set; }
    }

    private List<GradesListModel> gradesListModel = new();
    private List<GradesListModel> copyOfGradesListModel = new();


    private void CheckDifferences(out List<int> deletedIds, out List<GradeValueRecord> modifiedGrades)
    {
        deletedIds = new();
        modifiedGrades = new();
        foreach (var grade in copyOfGradesListModel)
        {
            var ogGrade = gradesListModel.SingleOrDefault(g => g.Id == grade.Id);
            if (ogGrade == null)
            {
                deletedIds.Add(grade.Id);
            }

            else if (grade.Grade != ogGrade.Grade)
            {
                modifiedGrades.Add(new GradeValueRecord(ogGrade.Id, ogGrade.Grade));
            }
        }
    }

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

            copyOfGradesListModel = gradesListModel.Select(g => new GradesListModel
            {
                Id = g.Id,
                Subject = g.Subject,
                Grade = g.Grade,
                Date = g.Date
            }).ToList();
        }
    }

    protected override async Task OnInitializedAsync()
    {
        var response = await Http.GetFromJsonAsync<List<SchoolClassDto>>("api/classes");
        if (response != null) schoolClasses = response;
    }

    private void RemoveItem(GradesListModel gradeToRemove)
    {
        gradesListModel.Remove(gradeToRemove);
    }

    private void CancelChanges()
    {
        gradesListModel = copyOfGradesListModel.Select(g => new GradesListModel
        {
            Id = g.Id,
            Subject = g.Subject,
            Grade = g.Grade,
            Date = g.Date
        }).ToList();
        StateHasChanged();
    }

    private async Task SubmitChanges()
    {
        Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
        CheckDifferences(out List<int> deletedIds, out List<GradeValueRecord> modifiedGrades);
        var gradebatchdto = new GradeBatchUpdateDto() { DeletedIds = deletedIds, UpdatedIdsAndValues = modifiedGrades };

        var response = await Http.PostAsJsonAsync("api/grades/batch-update", gradebatchdto);

        if (response.IsSuccessStatusCode)
        {
            Snackbar.Add($"{deletedIds.Count} grades were deleted\n{modifiedGrades.Count} grades were modified.",
                Severity.Success);
            copyOfGradesListModel = gradesListModel.Select(g => new GradesListModel
            {
                Id = g.Id,
                Subject = g.Subject,
                Grade = g.Grade,
                Date = g.Date
            }).ToList();
            StateHasChanged();
        }
        else Snackbar.Add($"Sikertelen", Severity.Error);
    }
}