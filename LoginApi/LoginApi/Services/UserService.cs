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
	public class UserService(IAsyncRepository<UserContext> repository)
	{

		public async Task CreateUser(User user)
		{
			var existingUser = await repository.GetItem<User>(x => x.Where(x => x.Username == user.Username))!;
			if (existingUser == null)
				await repository.AddItem(user);
		}


		public async Task<string> Login(UserDTO user)
		{
			var existingUser = await repository.GetItem<User>(x => x.Where(x => x.Username == user.Username))!;
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


		public async Task DeleteUser(Guid id)
		{
			await repository.RemoveItem<User>(x => x.Id == id);
		}

		public async Task DeleteAll()
		{
			await repository.RemoveItems<User>(x => x.Where(z => z.Username != string.Empty));
		}

		private string CreateToken(User user)
		{
			List<Claim> claims = new List<Claim>
			{
				new Claim(ClaimTypes.Name, user.Username)
			};


			var secretPath = Environment.GetEnvironmentVariable("secretPath");
			if (string.IsNullOrEmpty(secretPath) || !System.IO.File.Exists(secretPath))
			{
				Console.WriteLine("Secret path is not set or file does not exist");
			}

			var secretValue = System.IO.File.ReadAllText(secretPath!);

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretValue!));

			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

			var token = new JwtSecurityToken(
				claims: claims,
				expires: DateTime.Now.AddHours(5),
				signingCredentials: creds,
				issuer: "http://loginapi"
				);

			var jwt = new JwtSecurityTokenHandler().WriteToken(token);
			return jwt;
		}
	}
}