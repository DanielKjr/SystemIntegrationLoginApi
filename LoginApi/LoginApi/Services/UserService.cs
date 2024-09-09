using DK.GenericLibrary.Interfaces;
using LoginApi.Context;
using LoginApi.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace LoginApi.Services
{
	public class UserService(IAsyncRepository<UserContext> repository, IConfiguration configuration)
	{

		public async Task CreateUser(User user)
		{
			var existingUser = await repository.GetItem<User>(x => x.Where(x => x.Username == user.Username))!;
			if (existingUser == null)
				await repository.AddItem(user);
		}


		public async Task<string> Login(User user)
		{
			var existingUser = await repository.GetItem<User>(x => x.Where(x => x.Id == user.Id))!;
			if (existingUser != null)
			{
				if (BCrypt.Net.BCrypt.Verify(user.Password, existingUser.Password))
					return CreateToken(existingUser);
				else
					return "Username or password is incorrect.";
			}
			else
				return "Username or password is incorrect.";



		}


		public async Task<List<User>> GetAllUsers()
		{
			return await repository.GetAllItems<User>();
		}

		private string CreateToken(User user)
		{
			List<Claim> claims = new List<Claim>
			{
				new Claim(ClaimTypes.Name, user.Username)
			};

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["AppSettings:Token"]!));

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