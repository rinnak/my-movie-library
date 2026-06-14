using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using BCryptNet = BCrypt.Net.BCrypt;
using TVSeriesLibrary.Data;
using TVSeriesLibrary.Models;

namespace TVSeriesLibrary.Service;


public class AuthService : IAuthService
{
    private readonly ApplicationDb _context;
    private readonly IConfiguration _configuration;

    public AuthService(ApplicationDb context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task RegisterAsync(RegisterRequest request)
    {
        var userExists = await _context.Users.AnyAsync(u => u.Email == request.Email);
        if (userExists)
        {
            throw new Exception("Email already exists");
        }
        
        string passwordHash = BCryptNet.HashPassword(request.Password);

        var newUser = new User
        {
            Email = request.Email,
            PasswordHash = passwordHash,
            IsEmailConfirmed = true,
            EmailConfiramtionToken = null
        };
        
        _context.Users.Add(newUser);
        await _context.SaveChangesAsync();
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if (user == null)
        {
            throw new Exception("Неверный email или пароль");
        }

        bool isPasswordValid = BCryptNet.Verify(request.Password, user.PasswordHash);
        if (!isPasswordValid)
        {
            throw new Exception("Неверный email или пароль");
        }
        
        string token = GenerateJwtToken(user);

        return new AuthResponse()
        {
            Email = user.Email,
            Token = token,
        };
    }

    private string GenerateJwtToken(User user)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
        };

        var secretKey = _configuration["JwtSettings:SecretKey"];
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        
        // цифровая печать бэкенда
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var tokenOptions = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"],
            audience: _configuration["JwtSettings:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(14),
            signingCredentials: creds
        );
        
        return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
    }
}