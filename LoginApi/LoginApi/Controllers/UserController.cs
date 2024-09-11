using LoginApi.Models;
using LoginApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LoginApi.Controllers
{
	[Route("api/[controller]")]
	public class UserController(UserService _userService, IConfiguration configuration) : ControllerBase
	{



		[HttpPost]
		[Route("new")]
		public async Task AddUser([FromBody] User user)
		{
			var userPw = user.Password;
			var userPwHash = BCrypt.Net.BCrypt.HashPassword(userPw);
			user.Password = userPwHash;
			await _userService.CreateUser(user);
		}


		[HttpGet]
		[Route("all")]
		public async Task<List<User>> GetUsers()
		{
			return await _userService.GetAllUsers();
		}


		[HttpPost]
		[Route("login")]
		public async Task<string> Login([FromBody] UserDTO user)
		{
			return await _userService.Login(user);
		}
		//TODO tutorial gør det her fordi controllerbase har Ok men min funktioner er i service



		private string CreateToken(User user)
		{
			List<Claim> claims = new List<Claim>
			{
				new Claim(ClaimTypes.Name, user.Username)
			};

			var secret = Environment.GetEnvironmentVariable("secrets/jwtSecret");
			string secretValue;
			if (secret != null)
			{
				secretValue = System.IO.File.ReadAllText(secret); 
			}
			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));

			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

			var token = new JwtSecurityToken(
				claims: claims,
				expires: DateTime.Now.AddHours(5),
				signingCredentials: creds
				);

			var jwt = new JwtSecurityTokenHandler().WriteToken(token);
			return jwt;
		}
	}
}
