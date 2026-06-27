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

    



        [HttpPost("bulk-delete")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> BulkDeleteStudents(List<int> ids)
        {
            if (ids == null || !ids.Any()) return BadRequest("No selected items.");
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
                return StatusCode(500, "Error: " + e.Message);
            }
        }
    }
}