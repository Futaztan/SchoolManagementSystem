using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Server.Data;
using Shared.DTOs;
using Shared.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    //  [Authorize]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SchoolDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AuthController(UserManager<IdentityUser> userManager, SchoolDbContext context,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _context = context;
            _roleManager = roleManager;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("register-admin")]
        public async Task<IActionResult> RegisterAdmin(AdminDto dto)
        {
            string name = dto.Name;
            string pass = dto.Password;
            var user = new IdentityUser { UserName = name };
            var result = await _userManager.CreateAsync(user, pass);

            if (!result.Succeeded) throw new Exception("Failed to create admin user.");

            await _userManager.AddToRoleAsync(user, "Admin");
            return Ok();
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword(PasswordChangeModel pass)
        {
            string? userid = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userid is null) throw new Exception("User ID not found in claims.");
            var user = await _userManager.FindByIdAsync(userid);
            var result = await _userManager.ChangePasswordAsync(user!, pass.CurrentPassword, pass.NewPassword);
            if (result.Succeeded) return Ok();
            var errors = result.Errors.Select(e => e.Description);
            return BadRequest(new { Errors = errors });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("register-teacher")]
        public async Task<ActionResult<TeacherDto>> RegisterTeacher(TeacherDetailsDto dto)
        {
            var user = new IdentityUser { UserName = dto.Name, Email = dto.Email };
            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded) return BadRequest(result.Errors);
            await _userManager.AddToRoleAsync(user, "Teacher");
            var subjectNames = dto.TeachedSubjects.Select(ts => ts.Name).ToList();

            List<Subject> subjects = await _context.Subjects.Where(s => subjectNames.Contains(s.Name)).ToListAsync();

            Teacher teacher = new Teacher
                { Email = dto.Email, Name = dto.Name, TeachedSubjects = subjects, UserId = user.Id };
            _context.Teachers.Add(teacher);
            await _context.SaveChangesAsync();

            TeacherDetailsDto returndto = await _context.Teachers.ProjectToType<TeacherDetailsDto>()
                .SingleOrDefaultAsync(s => s.Id == teacher.Id);
            return Ok(returndto);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("register-student")]
        public async Task<ActionResult<StudentDto>> RegisterStudent(StudentDetailsDto dto)
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
                Console.WriteLine($"ERROR: {ex.Message}");
                if (ex.InnerException != null) Console.WriteLine($"INNER: {ex.InnerException.Message}");

                return StatusCode(500, $"Server error: {ex.Message}");
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _userManager.FindByNameAsync(dto.Name);
            if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
                return Unauthorized("Invalid username or password!");

            List<string> roles = (List<string>)await _userManager.GetRolesAsync(user);
            List<Claim> claims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // JWT Token generation
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("Nagyon_Hosszu_Es_Titkos_Kulcs_12345");
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return Ok(new { Token = tokenHandler.WriteToken(token), Role = roles[0] });
        }
    }
}