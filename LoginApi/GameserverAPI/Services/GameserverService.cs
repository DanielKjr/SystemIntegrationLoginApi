using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
namespace GameserverAPI.Services
{
	public class GameserverService
	{

		private List<int> freeServerPorts = new List<int> { 27015, 27016, 27017, 27018, 27019, 27020 };

		public GameserverService()
		{

		}


		public Task<string> VerifyJWT(string jwt)
		{
			var valid = false;
			var token = new JwtSecurityTokenHandler().ReadJwtToken(jwt);
			//something to verify it is valid?
			//var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetSection("AppSettings:Token").Value!));

			//var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
			if (valid)
				return Task.FromResult("some serverport");
			else
				return Task<string>.FromResult("invalid");
		}
	}
}
