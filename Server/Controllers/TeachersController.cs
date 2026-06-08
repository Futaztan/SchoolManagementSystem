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
    public class TeachersController : ControllerBase
    {
        private SchoolDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        public TeachersController(SchoolDbContext context, UserManager<IdentityUser> user)
        {
            _context = context;
            _userManager = user;
        }

        [HttpPost("bulk-delete")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> BulkDeleteTeachers(List<int> ids)
        {
            if (ids == null || !ids.Any()) return BadRequest("Nincs kijelölt elem.");
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                List<string> userids = await _context.Teachers.Where(s => ids.Contains(s.Id)).Select(s => s.UserId).ToListAsync();
                await _context.Teachers.Where(s => ids.Contains(s.Id)).ExecuteDeleteAsync();
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

        [HttpGet]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<ActionResult<List<TeacherDetailsDto>>> GetTeacher()
        {

            var list = await _context.Teachers.ProjectToType<TeacherDetailsDto>().ToListAsync();
           
            return list;
        }
        
        [Authorize(Roles = "Teacher")]
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
        public async Task<ActionResult<TeacherDetailsDto>> AddTeacher(TeacherDetailsDto dto)
        {
            var user = new IdentityUser { UserName = dto.Name, Email = dto.Email };
            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded) return NoContent();
            await _userManager.AddToRoleAsync(user, "Teacher");
            var subjectNames = dto.TeachedSubjects.Select(ts => ts.Name).ToList();

            List<Subject> subjects = await _context.Subjects.Where(s => subjectNames.Contains(s.Name)).ToListAsync();

            Teacher teacher = new Teacher { Email = dto.Email, Name = dto.Name, TeachedSubjects = subjects, UserId = user.Id };
            _context.Teachers.Add(teacher);
            await _context.SaveChangesAsync();

            TeacherDetailsDto returndto = await _context.Teachers.ProjectToType<TeacherDetailsDto>().SingleOrDefaultAsync(s => s.Id == teacher.Id);
            return Ok(returndto);
        }


      

    }
}
