using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using AuthService.Application.Interfaces;
using AuthService.Domain.Entities;

namespace AuthService.Infrastructure.Security;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly IConfiguration _configuration;

    public JwtTokenGenerator(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(User user)
    {
        // Read JWT settings
        var jwtKey = _configuration["Jwt:Key"]
            ?? throw new Exception("JWT Key is missing");

        var issuer = _configuration["Jwt:Issuer"]
            ?? throw new Exception("JWT Issuer is missing");

        var audience = _configuration["Jwt:Audience"]
            ?? throw new Exception("JWT Audience is missing");

        // Convert key to bytes
        var keyBytes = Encoding.UTF8.GetBytes(jwtKey);

        // HS256 requires minimum 256-bit key (32 bytes)
        if (keyBytes.Length < 32)
        {
            throw new Exception(
                "JWT Key must be at least 32 bytes long");
        }

        // Claims
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),

            // Optional additional claims
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // Create security key
        var securityKey = new SymmetricSecurityKey(keyBytes);

        // Create signing credentials
        var credentials = new SigningCredentials(
            securityKey,
            SecurityAlgorithms.HmacSha256);

        // Create token
        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: credentials);

        // Return serialized token
        return new JwtSecurityTokenHandler()
            .WriteToken(token);
    }

    // Optional helper method to generate secure random JWT keys
    public static string GenerateSecureJwtKey()
    {
        return Convert.ToBase64String(
            RandomNumberGenerator.GetBytes(32));
    }
}