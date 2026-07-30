using CourseApi.Data;
using CourseApi.DTOs;
using CourseApi.Models;
using CourseApi.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CourseApi.Services;


namespace CourseApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly PasswordHasher<AppUser> _passwordHasher;
        private readonly JwtService _jwtService;

        public AuthController( AppDbContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
            _passwordHasher = new PasswordHasher<AppUser>();
        }



        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {

            var existingUser = await _context.AppUsers
                .FirstOrDefaultAsync(u => u.Email == dto.Email);


            if (existingUser != null)
            {
                return BadRequest("Email already exists");
            }


            var user = new AppUser
            {
                Username = dto.Username,
                Email = dto.Email,
                Role = CourseApi.Enums.Role.Student
            };


            user.PasswordHash =
                _passwordHasher.HashPassword(
                    user,
                    dto.Password
                );


            _context.AppUsers.Add(user);

            await _context.SaveChangesAsync();


            return Ok(new
            {
                message = "User registered successfully"
            });
        }



        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _context.AppUsers
                .FirstOrDefaultAsync(u => u.Email == dto.Email);


            if (user == null)
            {
                return Unauthorized("Invalid email or password");
            }


            var passwordResult =
                _passwordHasher.VerifyHashedPassword(
                    user,
                    user.PasswordHash,
                    dto.Password
                );


            if (passwordResult == PasswordVerificationResult.Failed)
            {
                return Unauthorized("Invalid email or password");
            }


            var token = _jwtService.GenerateToken(user);


            return Ok(new AuthResponseDto
            {
                Token = token,
                Username = user.Username,
                Email = user.Email,
                Role = user.Role.ToString()
            });
        }




    }
}