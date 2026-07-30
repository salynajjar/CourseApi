using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CourseApi.Models;
using Microsoft.IdentityModel.Tokens;

namespace CourseApi.Services
{
    public class JwtService
    {
        private readonly IConfiguration _configuration;


        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }



        public string GenerateToken(AppUser user)
        {
            var claims = new[]
            {
                new Claim(
                    JwtRegisteredClaimNames.Sub,
                    user.Id.ToString()
                ),

                new Claim(
                    JwtRegisteredClaimNames.Email,
                    user.Email
                ),

                new Claim(
                    ClaimTypes.Name,
                    user.Username
                ),

                new Claim(
                    ClaimTypes.Role,
                    user.Role.ToString()
                )
            };


            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    _configuration["Jwt:Key"]!
                )
            );


            var credentials =
                new SigningCredentials(
                    key,
                    SecurityAlgorithms.HmacSha256
                );


            var token = new JwtSecurityToken(

                issuer:
                    _configuration["Jwt:Issuer"],

                audience:
                    _configuration["Jwt:Audience"],

                claims: claims,

                expires:
                    DateTime.UtcNow.AddMinutes(
                        double.Parse(
                            _configuration["Jwt:DurationInMinutes"]!
                        )
                    ),

                signingCredentials:
                    credentials
            );


            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }
    }
}