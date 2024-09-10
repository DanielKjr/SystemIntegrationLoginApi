namespace SessionAPI.Services
{
	public class SessionService
	{
		public Dictionary<string, string> Users = new Dictionary<string, string>() { { "username", "12311" } };


		public Task AddUser(string username, string password)
		{
			Users.Add(username, password);
			return Task.CompletedTask;
		}

		public Task<Dictionary<string, string>> AddUsers(Dictionary<string, string> users)
		{
			return Task.FromResult(Users = users);
		}


		public Task Remove(string username)
		{
			Users.Remove(username);
			return Task.CompletedTask;
		}

		public Task<Dictionary<string, string>> GetAll()
		{
			return Task.FromResult(Users);
		}

		public Task Update(string user, string serverPort)
		{
			Users[user] = serverPort;
			return Task.CompletedTask;
		}
	}
}
