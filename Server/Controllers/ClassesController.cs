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
    public class ClassesController : ControllerBase
    {
        private SchoolDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        public ClassesController(SchoolDbContext context, UserManager<IdentityUser> user)
        {
            _context = context;
            _userManager = user;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddClass(SchoolClassDto dto)
        {
            SchoolClass schoolClass = new SchoolClass { Name = dto.Name, Year = dto.Year };
            _context.SchoolClasses.Add(schoolClass);
            await _context.SaveChangesAsync();

            SchoolClassDto returnDto = await _context.SchoolClasses.ProjectToType<SchoolClassDto>().SingleOrDefaultAsync(s => s.Id == schoolClass.Id);
            return Ok(returnDto);
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Teacher")]
        public async Task<ActionResult<List<SchoolClassDto>>> GetClasses()
        {
            var list = await _context.SchoolClasses.ProjectToType<SchoolClassDto>().ToListAsync();
            return Ok(list);
        }




        [HttpPost("bulk-delete")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> BulkDeleteClasses(List<int> ids)
        {
            if (ids == null || !ids.Any()) return BadRequest("No selected items");
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _context.SchoolClasses.Where(s => ids.Contains(s.Id)).ExecuteDeleteAsync();
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
