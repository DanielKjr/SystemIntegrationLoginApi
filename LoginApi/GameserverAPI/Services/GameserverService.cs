using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
namespace GameserverAPI.Services
{
	public class GameserverService
	{

		private List<int> freeServerPorts = new List<int> { 27015, 27016, 27017, 27018, 27019, 27020 };
		


		public Task<string> VerifyJWT(string jwt)
		{
			var secretPath = Environment.GetEnvironmentVariable("secretPath");
			if (string.IsNullOrEmpty(secretPath) || !System.IO.File.Exists(secretPath))
			{
				Console.WriteLine("Secret path is not set or file does not exist");
			}
			


			var secretValue = System.IO.File.ReadAllText(secretPath!);
			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretValue!));

			var tokenHandler = new JwtSecurityTokenHandler();
			var validationParameters = new TokenValidationParameters
			{
				ValidateIssuer = true,           // Skip validation of the token's issuer
				ValidateAudience = false,         // Skip validation of the token's audience
				ValidateLifetime = true,          // Ensure the token has not expired
				ValidateIssuerSigningKey = true,  // Validate the signing key
				IssuerSigningKey = key, // Provide the shared secret key for validation
				ValidIssuers = new[] { "http://loginapi" }  // valid issuer, should be hidden of course
			};

			try
			{
				// Validate the token and return the principal (claims)
				var principal = tokenHandler.ValidateToken(jwt, validationParameters, out SecurityToken validatedToken);

				// You can access token properties like claims and expiration here
				var jwtToken = (JwtSecurityToken)validatedToken;
				

				// If needed, check specific claims or other token properties manually
				var usernameClaim = principal.FindFirst(ClaimTypes.Name)?.Value;

				return Task.FromResult("Registration Succeeded");
			}
			catch (SecurityTokenException ex)
			{
				// Token validation failed
				Console.WriteLine($"Token validation failed: {ex.Message}");
				return Task.FromResult("Failed");
			}
		}
	}
}
