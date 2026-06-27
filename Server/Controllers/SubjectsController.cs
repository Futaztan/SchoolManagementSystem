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
    public class SubjectsController : ControllerBase
    {
        private SchoolDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        public SubjectsController(SchoolDbContext context, UserManager<IdentityUser> user)
        {
            _context = context;
            _userManager = user;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<SubjectDto>> AddSubject(SubjectDto dto)
        {
            Subject subject = new Subject { Name = dto.Name };
            _context.Subjects.Add(subject);
            await _context.SaveChangesAsync();

            SubjectDto returnDto = await _context.Subjects.ProjectToType<SubjectDto>().SingleOrDefaultAsync(s => s.Id == subject.Id);
            return Ok(returnDto);
        }
        [HttpGet]
        [Authorize(Roles = "Admin, Teacher")]
        public async Task<ActionResult<List<SubjectDto>>> GetSubjects()
        {
            var list = await _context.Subjects.ProjectToType<SubjectDto>().ToListAsync();
            return list;
        }



        [HttpPost("bulk-delete")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> BulkDeleteSubjects(List<int> ids)
        {
            if (ids == null || !ids.Any()) return BadRequest("No selected items.");
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _context.Subjects.Where(s => ids.Contains(s.Id)).ExecuteDeleteAsync();
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
