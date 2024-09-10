using Microsoft.AspNetCore.Mvc;
using SessionAPI.Services;

namespace SessionAPI.Controllers
{
	[Route("api/v1/[controller]")]
	public class SessionController : ControllerBase
	{


		private readonly SessionService _sessionService;

		public SessionController(SessionService sessionService)
		{
			_sessionService = sessionService;
		}

		[HttpGet("currentUsers")]
		public IActionResult GetUsers()
		{
			return Ok(_sessionService.Users);
		}


		[HttpPost("addUser")]
		public IActionResult AddUser([FromBody] string user)
		{
			
			return Ok(_sessionService.AddUser(user, ""));
		}

		[HttpPost("addUsers")]
		public IActionResult AddUsers([FromBody] Dictionary<string,string> users)
		{
			return Ok(_sessionService.AddUsers(users));
		}

		[HttpDelete("remove/{user}")]
		public IActionResult RemoveUser(string user)
		{

			
			return Ok(_sessionService.Remove(user));
		}

		[HttpPost("update/{user}")]
		public IActionResult UpdateUser(string user, [FromBody]string serverPort)
		{
			
			return Ok(_sessionService.Update(user, serverPort));
		}

	}
}
