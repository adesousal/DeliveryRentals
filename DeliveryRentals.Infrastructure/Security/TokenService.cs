using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DeliveryRentals.Infrastructure.Security
{
	public static class TokenService
	{
		public static string GenerateToken(string username, string role, string jwtKey)
		{
			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken(
				claims: new[]
				{
				new Claim(ClaimTypes.Name, username),
				new Claim(ClaimTypes.Role, role)
				},
				expires: DateTime.UtcNow.AddHours(1),
				signingCredentials: creds);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}
	}
}
