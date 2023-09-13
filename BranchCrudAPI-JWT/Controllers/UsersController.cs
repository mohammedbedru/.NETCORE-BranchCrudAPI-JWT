using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BranchCrudAPI_JWT.Data;
using BranchCrudAPI_JWT.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using NuGet.Configuration;

namespace BranchCrudAPI_JWT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly BranchCrudAPI_JWTContext _context;
        private readonly IConfiguration _configuration;
        public UsersController(BranchCrudAPI_JWTContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [Authorize]
        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok(new { Message = "restricted content" });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User model)
        {
            // Check if the username already exists
            var existingUser = await _context.User.FirstOrDefaultAsync(u => u.Username == model.Username);
            if (existingUser != null)
            {
                return BadRequest(new { error = "Username is already taken." });
            }

            // Hash the password
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);

            // Create and add the new user
            if (ModelState.IsValid)
            {
                var user = new User { Username = model.Username, Email=model.Email, Password = hashedPassword }; 
                _context.User.Add(user);
                await _context.SaveChangesAsync();
                return Ok(new { Message = "Registration successful" });
            }

            return BadRequest(new { Message = "Invalid registration data" });
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginViewModel model)
        {
            var user = _context.User.SingleOrDefault(u => u.Username == model.Username); 

            if (user != null)
            {
                // Verify the hashed password
                bool isPasswordValid = BCrypt.Net.BCrypt.Verify(model.Password, user.Password);

                if (isPasswordValid)
                {
                    var token = GenerateJwtToken(user);
                    //return Ok(new { Token = token });
                    return Ok(new
                    {
                        id = user.Id,
                        username = user.Username,
                        email = user.Email,
                        accessToken = token
                    });
                }

                
            }

            return Unauthorized(new { Message = "Invalid username or password" });
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["AppSettings:Secret"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
            new Claim(ClaimTypes.Name, user.Username),
                    // Add more claims as needed
                }),
                Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["AppSettings:ExpirationInMinutes"])),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }


    }
}
