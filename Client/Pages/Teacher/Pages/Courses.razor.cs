using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using Shared.DTOs;
using Shared.Entities;

namespace Client.Pages.Teacher.Pages;

public partial class Courses
{
    [CascadingParameter] private Task<AuthenticationState>? AuthStateTask { get; set; }
	private NewCourseModel courseModel = new NewCourseModel();
	private List<CourseDto> courses = new List<CourseDto>();
	private List<SubjectDto> subjects = new List<SubjectDto>();
	private List<SchoolClassDto> schoolClasses = new List<SchoolClassDto>();
	private List<RoomDto> rooms = new List<RoomDto>();
	private HashSet<CourseDto> selectedCourses = new HashSet<CourseDto>();
	private List<int> LessonsNumbers = Enumerable.Range(0, 10).ToList();

	private CourseDto? GetCourseForSlot(DayOfWeek dayOfWeek, int lesson)
	{
		return courses.FirstOrDefault(c => c.CourseSchedules.Any(s =>
			s.DayOfWeek == dayOfWeek && s.LessonNumber == lesson));
	}

	protected override async Task OnInitializedAsync()
	{
		var response = await Http.GetFromJsonAsync<List<SchoolClassDto>>("api/classes");
		if (response != null) schoolClasses = response;

		var response2 = await Http.GetFromJsonAsync<List<SubjectDto>>("api/subjects");
		if (response2 != null) subjects = response2;

		var response3 = await Http.GetFromJsonAsync<List<RoomDto>>("api/rooms");
		if (response3 != null) rooms = response3;

		var response4 = await Http.GetFromJsonAsync<List<CourseDto>>("api/courses");
		if (response4 != null) courses = response4;
		
	}

	private async Task HandleAdd()
	{
		//var authState = await authStateTask;
		//var user = authState.User;

		var response = await Http.PostAsJsonAsync("api/courses", courseModel);
		if (response.IsSuccessStatusCode)
		{
			CourseDto newCourse = await response.Content.ReadFromJsonAsync<CourseDto>();
			if (newCourse != null)
			{
				courses.Add(newCourse);
				Snackbar.Add($"Sikeres hozzáadás", Severity.Success);
			}
		}
	}

	private async Task OnDeleteSelected()
	{
		Snackbar.Configuration.PositionClass = Defaults.Classes.Position.BottomLeft;
		var idsToDelete = selectedCourses.Select(s => s.Id).ToList();
		if (!idsToDelete.Any()) return;
		var response = await Http.PostAsJsonAsync("api/classes/bulk-delete", idsToDelete);

		if (response.IsSuccessStatusCode)
		{
			courses.RemoveAll(s => idsToDelete.Contains(s.Id));
			selectedCourses.Clear();
			Snackbar.Add($"{idsToDelete.Count} tárgy törölve.", Severity.Success);
			StateHasChanged();
		}
		else Snackbar.Add($"Sikertelen", Severity.Error);
	}

	private string searchString = "";

	private bool FilterFunc(CourseDto course)
	{
		if (string.IsNullOrWhiteSpace(searchString))
			return true;
		if (course.SchoolClassName.Contains(searchString, StringComparison.OrdinalIgnoreCase))
			return true;
		if (course.Subject.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
			return true;


		return false;
	}
}