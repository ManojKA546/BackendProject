

using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using MongoDB.Driver;
using BCrypt.Net;
using System.Text;



public class Usercontroller:ControllerBase
{
    private readonly MongoDBService _mongoDBService;
    private readonly IConfiguration _configuration;

     public Usercontroller(MongoDBService mongoDBService,IConfiguration configuration)
    {
        _mongoDBService = mongoDBService;  
        _configuration = configuration; 
    }

    [HttpPost("signup")]
    public IActionResult Signup(User user)
    {
        // Check if the username already exists
        var existingUser = _mongoDBService.GetUserByUsername(user.Username);
        if (existingUser != null)
        {
            return BadRequest("Username already exists.");
        }

        // Hash the password before storing it
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);

        // Save the user to the database
        _mongoDBService.AddUser(user);

        return Ok("Signup successful.");
        
    }


    [HttpPost("login")]
    public IActionResult Login([FromBody] User loginRequest)
    {
        // Find the user in the database
        var user = _mongoDBService.GetUserByUsername(loginRequest.Username);
        
        if (user == null || !BCrypt.Net.BCrypt.Verify(loginRequest.PasswordHash, user.PasswordHash))
        {
            return Unauthorized("Invalid username or password.");
        }
        
        // Generate JWT token
        var token = GenerateJwtToken(user);

        return Ok(new { Token = token });
    }

        private string GenerateJwtToken(User user)
    {
        // Get the secret key from configuration
        var secretKey =_configuration["Jwt:SecretKey"];
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Create claims
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // Generate token
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
