using System.Globalization;
using System.Net.Http.Json;
using System.Security.Claims;
using Shared.DTOs;
using Shared.Entities;

namespace Client.Pages.Teacher.Pages;

public partial class Grades
{
	private List<int> Months = Enumerable.Range(1, 12).ToList();
	private Dictionary<StudentDto, string> newGradeDic = new();
	private List<SchoolClassDto> schoolClasses = new List<SchoolClassDto>();
	
	private SelectionModel selectionModel = new();
	private string? teacherName;
	private string? teacherId;
	private string? submittedData;
	private bool IsSubmited = false;
	private SchoolClassDto? SelectedClass => schoolClasses.SingleOrDefault(c => c.Id == selectionModel.SelectedClassId);
	private CourseDto? SelectedCourse => schoolClasses.SelectMany(s => s.Courses).SingleOrDefault(c => c.Id == selectionModel.SelectedCourseId);
	private List<StudentDto>? students;
	
	private class SelectionModel
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
	
	
	private string GetSelectedGrade(StudentDto student)
	{
		if (newGradeDic.TryGetValue(student, out var jegy))
		{
			return jegy;
		}

		return "-";
	}
	
	private void OnGradeChanged(StudentDto student, string value)
	{
		newGradeDic[student] = value;
	}
	private string GetMonthName(int monthNumber)
	{
		return CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(monthNumber);
	}

	private List<GradeDto> GetMonthlyData(StudentDto student, int monthNumber)
	{
		var g = student.Grades.Where(g => g.Date.Month == monthNumber).ToList();
		return g;
	}
	private double GetStudentAverage(StudentDto student)
	{
		int sum = 0;
		int count = 0;
		foreach (var month in Months)
		{
			var gradesList = GetMonthlyData(student, month);
			foreach (var grade in gradesList)
			{
				sum += grade.Value;
				count++;
			}
		}

		if (count == 0) return 0;
		return Math.Round((double)sum / count, 2);
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