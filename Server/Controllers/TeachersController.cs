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
            if (ids == null || !ids.Any()) return BadRequest("No selected items.");
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

                return StatusCode(500, "Error: " + e.Message);
            }

        }

        [HttpGet]
        [Authorize(Roles = "Admin,Teacher")]
        public async Task<ActionResult<List<TeacherDetailsDto>>> GetTeacher()
        {

            var list = await _context.Teachers.ProjectToType<TeacherDetailsDto>().ToListAsync();
           
            return list;
        }

       


      

    }
}
