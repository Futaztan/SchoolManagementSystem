using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Server.Data;
using Server.Migrations;
using Shared.DTOs;
using Shared.Entities;
using System.Security.Claims;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CoursesController : ControllerBase
    {
        private SchoolDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CoursesController(SchoolDbContext context, UserManager<IdentityUser> user)
        {
            _context = context;
            _userManager = user;
        }

        [HttpPost]
        [Authorize(Roles = "Teacher, Admin")]
        public async Task<ActionResult<CourseDto>> AddCourse(NewCourseModel dto)
        {
            string teacherid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Teacher teacher = await _context.Teachers.SingleOrDefaultAsync(t => t.UserId == teacherid);

            Course course = new Course
                { SchoolClassId = dto.SchoolClassId, SubjectId = dto.SubjectId, TeacherId = teacher.Id };
            CourseSchedule courseSchedule = new CourseSchedule
            {
                Course = course, DayOfWeek = (DayOfWeek)dto.DayOfWeek, LessonNumber = dto.LessonNumber,
                RoomId = dto.RoomId
            };


            course.CourseSchedules.Add(courseSchedule);
            _context.Courses.Add(course);

            await _context.SaveChangesAsync();

            CourseDto returnDto = await _context.Courses.ProjectToType<CourseDto>()
                .SingleOrDefaultAsync(c => c.Id == course.Id);
            return Ok(returnDto);
        }


        [HttpGet]
        public async Task<ActionResult<List<CourseDto>>> GetCourses([FromQuery] int? cId)
        {
            if (User.IsInRole("Student"))
            {
                string id = User.FindFirstValue(ClaimTypes.NameIdentifier);
                int clasid = await _context.Students
                    .Where(s => s.UserId == id)
                    .Select(s => s.SchoolClassId)
                    .SingleAsync();

                var courses = await _context.Courses
                    .Where(c => c.SchoolClassId == clasid)
                    .ProjectToType<CourseDto>().ToListAsync();
                return Ok(courses);
            }

            if (cId != null)
            {
                SchoolClass? schoolClass = await _context.SchoolClasses.SingleOrDefaultAsync(c => c.Id == cId);
                if (schoolClass == null) return BadRequest("Az osztály nem található az adatbázisban.");
                var courses = await _context.Courses.Where(c => c.SchoolClassId == schoolClass.Id).ToListAsync();
                return Ok(courses);
            }

            if (User.IsInRole("Teacher"))
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var teacherid = await _context.Teachers
                    .Where(t=> t.UserId==userId)
                    .Select(t=> t.Id)
                    .SingleAsync();
                
                var teacherCourses = await _context.Courses.Where(c => c.TeacherId == teacherid).ProjectToType<CourseDto>().ToListAsync();
                return Ok(teacherCourses);
            }

            if (User.IsInRole("Admin"))
            {
                var allCourses = await _context.Courses
                    .ProjectToType<CourseDto>()
                    .ToListAsync();

                return Ok(allCourses);
            }

            return BadRequest();
        }


        //TODO

        //[HttpPost("bulk-delete")]
        //[Authorize(Roles = "Admin")]
        //public async Task<IActionResult> BulkDeleteRooms(List<int> ids)
        //{
        //    if (ids == null || !ids.Any()) return BadRequest("Nincs kijelölt elem.");
        //    using var transaction = await _context.Database.BeginTransactionAsync();
        //    try
        //    {
        //        await _context.Courses.Where(s => ids.Contains(s.Id)).ExecuteDeleteAsync();
        //        await transaction.CommitAsync();
        //        return Ok();
        //    }
        //    catch (Exception e)
        //    {
        //        return StatusCode(500, "Hiba történt: " + e.Message);
        //    }

        //}
    }
}