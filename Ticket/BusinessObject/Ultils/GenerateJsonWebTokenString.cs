using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BusinessObject.Commons;
using DataAccessObject.Entities;
using Microsoft.IdentityModel.Tokens;

namespace BusinessObject.Ultils;

public static class GenerateJsonWebTokenString
{
    public static string GenerateJsonWebToken(this User user, AppConfiguration appSettingConfiguration, string secretKey, DateTime now)
    {
        // Ensure the secret key is 32 bytes long
        if (Encoding.UTF8.GetBytes(secretKey).Length < 32)
        {
            // Adjust key length to 32 bytes (256 bits) using padding if necessary
            secretKey = secretKey.PadRight(32, '0');
        }

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        // Create claims for the token
        var claims = new[]
        {
            new Claim("Id", user.Id.ToString()),
            new Claim("Email", user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString()), // Convert Role enum to string
        };

        // Create the token
        var token = new JwtSecurityToken(
            issuer: appSettingConfiguration.JWTSection.Issuer,
            audience: appSettingConfiguration.JWTSection.Audience,
            claims: claims,
            expires: now.AddHours(3),
            signingCredentials: credentials);

        // Return the token as a string
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}