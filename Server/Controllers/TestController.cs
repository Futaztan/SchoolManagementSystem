using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Server.Data;
using Shared.DTOs;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SchoolDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;

        public TestController(UserManager<IdentityUser> userManager, SchoolDbContext context, RoleManager<IdentityRole>roleManager)
        {
            _userManager = userManager;
            _context = context;
            _roleManager = roleManager;
        }
        [HttpGet("addroles")]
        public async Task<IActionResult> Test()
        {
            if (!await _roleManager.RoleExistsAsync("Student"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Student"));
            }
            if (!await _roleManager.RoleExistsAsync("Teacher"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Teacher"));
            }
            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
            }
            return Ok();

        }

    }
}
