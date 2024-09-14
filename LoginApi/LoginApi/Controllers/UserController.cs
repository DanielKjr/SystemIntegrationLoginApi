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
	public class UserController(UserService _userService) : ControllerBase
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
		

		[HttpDelete("delete/{id}")]
		public async Task DeleteUser(Guid id)
		{
			await _userService.DeleteUser(id);
		}

		[HttpDelete("delete/all")]
		public async Task DeleteAll()
		{
			await _userService.DeleteAll();
		}

	}
}
