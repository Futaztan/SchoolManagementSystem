using System.Runtime.InteropServices.JavaScript;
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
    public class GradesController : ControllerBase
    {
        private SchoolDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public GradesController(SchoolDbContext context, UserManager<IdentityUser> user)
        {
            _context = context;
            _userManager = user;
        }

        [HttpPost]
        [Authorize(Roles = "Teacher, Admin")]
        public async Task<ActionResult> AddGrade(List<GradeDto> dtos)
        {
            // string teacherid = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            // Teacher teacher = await _context.Teachers.SingleOrDefaultAsync(t => t.UserId == teacherid);
            foreach (var dto in dtos)
            {
                Grade grade = new Grade
                {
                    Date = dto.Date, StudentId = dto.StudentId, Value = dto.Value,
                    CourseId = dto.CourseId
                };
                _context.Grades.Add(grade);
            }


            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult<List<StudentDto>>> GetGrades([FromQuery] int? sId, [FromQuery] int? cId)
        {
            Student? student;
            List<Student> students;
            if (User.IsInRole("Student"))
            {
                string id = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
                student = await _context.Students.Include(s => s.Grades).SingleAsync(s => s.UserId == id);
                students = new List<Student> { student };
            }
            else if (sId != null)
            {
                student = await _context.Students.Include(s => s.Grades).SingleOrDefaultAsync(s => s.Id == sId);
                if (student == null) return NotFound("A diák nem található az adatbázisban.");
                students = new List<Student> { student };
            }
            else if (cId != null)
            {
                students = await _context.Students.Include(s => s.Grades).Where(c => c.SchoolClassId == cId)
                    .ToListAsync();
                if (students.Count == 0) return BadRequest("Az osztály nem található az adatbázisban.");
            }
            else
            {
                return BadRequest("Hibás paraméterek");
            }

            // var gradesDto = await _context.Grades.ProjectToType<GradeDto>()
            //     .Where(g => g.StudentId == student.Id && g.CourseId == courseId).ToListAsync();
            var ids = students.Select(s => s.Id).ToList();
            var dto = await _context.Students.ProjectToType<StudentDto>().Where(s => ids.Contains(s.Id))
                .ToListAsync();
            return Ok(dto);
        }

        [HttpPost("batch-update")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> BatchUpdateGrades([FromBody] GradeBatchUpdateDto payload)
        {
            List<GradeValueRecord> modifiedGrades = payload.UpdatedIdsAndValues;
            List<int> deletedIds = payload.DeletedIds;

            if (modifiedGrades.Any())
            {
                var modifiedIds = modifiedGrades.Select(m => m.GradeId).ToList();

                var gradesToUpdate = await _context.Grades
                    .Where(g => modifiedIds.Contains(g.Id))
                    .ToListAsync();

                foreach (var (id, newValue) in modifiedGrades)
                {
                    var grade = gradesToUpdate.SingleOrDefault(s => s.Id == id);
                    if (grade != null)
                    {
                        grade.Value = newValue;
                    }
                }
            }


            if (deletedIds.Any())
            {
                var list = await _context.Grades.Where(g => deletedIds.Contains(g.Id)).ToListAsync();
                _context.Grades.RemoveRange(list);
            }


            await _context.SaveChangesAsync();
            return Ok();
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