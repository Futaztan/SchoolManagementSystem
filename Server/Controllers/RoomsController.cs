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
    public class RoomsController : ControllerBase
    {
        private SchoolDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        public RoomsController(SchoolDbContext context, UserManager<IdentityUser> user)
        {
            _context = context;
            _userManager = user;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<RoomDto>> AddRoom(RoomDto dto)
        {
            Room room = new Room { Name = dto.Name };
            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();

            RoomDto returnDto = await _context.Rooms.ProjectToType<RoomDto>().SingleOrDefaultAsync(s => s.Id == room.Id);
            return Ok(returnDto);
        
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Teacher")]
        public async Task<ActionResult<List<RoomDto>>> GetRooms()
        {
            var list = await _context.Rooms.Select(s => new RoomDto { Name = s.Name, Id = s.Id }).ToListAsync();
            return list;
        }

        [HttpPost("bulk-delete")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> BulkDeleteRooms(List<int> ids)
        {
            if (ids == null || !ids.Any()) return BadRequest("Nincs kijelölt elem.");
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await _context.Rooms.Where(s => ids.Contains(s.Id)).ExecuteDeleteAsync();
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
