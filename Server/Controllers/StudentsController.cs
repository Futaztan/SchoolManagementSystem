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
    public class StudentsController : ControllerBase
    {
        private SchoolDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public StudentsController(SchoolDbContext context, UserManager<IdentityUser> user)
        {
            _context = context;
            _userManager = user;
        }


        [HttpGet]
        [Authorize(Roles = "Admin,Student")]
        public async Task<ActionResult<List<StudentDetailsDto>>> GetStudent()
        {
            if (User.IsInRole("Student"))
            {
                string userid = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
                var student = await _context.Students.Where(s => s.UserId == userid).ProjectToType<StudentDetailsDto>()
                    .SingleAsync();
                return new List<StudentDetailsDto>() { student };
            }

            var list = await _context.Students.ProjectToType<StudentDetailsDto>().ToListAsync();
            return list;
        }

        [Authorize(Roles = "Student")]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(PasswordChangeModel pass)
        {
            
            string userid = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var user = await _userManager.FindByIdAsync(userid);
            var result = await _userManager.ChangePasswordAsync(user!, pass.CurrentPassword, pass.NewPassword);
            if (result.Succeeded) return Ok();
            var errors = result.Errors.Select(e => e.Description);
            return BadRequest(new { Errors = errors });
            
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<StudentDetailsDto>> AddStudent(StudentDetailsDto dto)
        {
            try
            {
                var user = new IdentityUser { UserName = dto.Name, Email = dto.Email };
                var result = await _userManager.CreateAsync(user, dto.Password);
                if (!result.Succeeded) return StatusCode(500, result);
                await _userManager.AddToRoleAsync(user, "Student");
                //var schoolclass = await _context.SchoolClasses.SingleOrDefaultAsync(c => c.Id == dto.SchoolClassId);

                Student student = new Student
                {
                    BirthDate = (DateOnly)dto.BirthDate, Name = dto.Name, SchoolClassId = dto.SchoolClassId,
                    Email = dto.Email, UserId = user.Id
                };
                _context.Students.Add(student);
                await _context.SaveChangesAsync();

                StudentDto returndto = await _context.Students.ProjectToType<StudentDto>()
                    .SingleOrDefaultAsync(s => s.Id == student.Id);

                //StudentDto returndto = new StudentDto { Id = s.Id, BirthDate = s.BirthDate, Email = s.Email, Grades = s.Grades, Name = s.Name, SchoolClassId = s.SchoolClassId, SchoolClass = s.Class };
                return Ok(returndto);
            }

            catch (Exception ex)
            {
                Console.WriteLine($"HIBA: {ex.Message}");
                if (ex.InnerException != null) Console.WriteLine($"INNER: {ex.InnerException.Message}");

                return StatusCode(500, $"Szerver hiba: {ex.Message}");
            }
        }


        [HttpPost("bulk-delete")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> BulkDeleteStudents(List<int> ids)
        {
            if (ids == null || !ids.Any()) return BadRequest("Nincs kijelölt elem.");
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                List<string> userids = await _context.Students.Where(s => ids.Contains(s.Id)).Select(s => s.UserId)
                    .ToListAsync();
                await _context.Students
                    .Where(s => ids.Contains(s.Id))
                    .ExecuteDeleteAsync();
                foreach (var userId in userids)
                {
                    var user = await _userManager.FindByIdAsync(userId);
                    if (user != null) await _userManager.DeleteAsync(user);
                }


                await transaction.CommitAsync();
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, "Hiba történt: " + e.Message);
            }
        }
    }
}