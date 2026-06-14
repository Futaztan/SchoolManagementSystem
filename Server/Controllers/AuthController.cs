using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Server.Data;
using Shared.DTOs;
using Shared.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SchoolDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AuthController(UserManager<IdentityUser> userManager, SchoolDbContext context, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _context = context;
            _roleManager = roleManager;
        }
        [HttpGet("addadmin")]
        public async Task<IActionResult> AddAdmin()
        {
            string name = "admin2";
            string pass = "pass";
            // 1. Felhasználó létrehozása az Identity-ben
            var user = new IdentityUser { UserName = name};
            var result = await _userManager.CreateAsync(user, pass);

            if (!result.Succeeded) throw new Exception();

            await _userManager.AddToRoleAsync(user, "Admin");
            return Ok();
        }
        
       
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


        //[HttpPost("register")]
        // public async Task<IActionResult> Register()
        // {
        //    // 1. Felhasználó létrehozása az Identity-ben
        //    var dto = new RegisterDto { BirthDate = new DateOnly(2000, 1, 1), Class = new SchoolClass { Name = "11b" }, Email = "asdsadads@asdad.hu", Name = "janos", Password = "pass" };
        //     var user = new IdentityUser { UserName = dto.Name, Email = dto.Email };
        //     var result = await _userManager.CreateAsync(user, dto.Password);

        //     if (!result.Succeeded) return BadRequest(result.Errors);

        //    await _userManager.AddToRoleAsync(user, "Student");
        //    // 2. Diák létrehozása és összekötése az IdentityUserrel
        //    var student = new Student
        //    {
        //        Name = dto.Name,
        //        Class = dto.Class,
        //         Email = dto.Email,
        //        BirthDate = dto.BirthDate,
              
        //        UserId = user.Id // Itt történik az összekapcsolás!
        //    };

        //     _context.Students.Add(student);
        //     await _context.SaveChangesAsync();

        //     return Ok("Sikeres regisztráció!");
        // }
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
          
            var user = await _userManager.FindByNameAsync(dto.Name);
            if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
                return Unauthorized("Hibás email vagy jelszó!");

            List<string> roles = (List<string>) await _userManager.GetRolesAsync(user);
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                
            };
     
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // JWT Token generálása
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("Nagyon_Hosszu_Es_Titkos_Kulcs_12345");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return Ok(new { Token = tokenHandler.WriteToken(token), Role =  roles[0] });
        }
    }
}
