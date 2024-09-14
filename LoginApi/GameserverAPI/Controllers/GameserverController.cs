using GameserverAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace GameserverAPI.Controllers
{
	[Route("v1/[controller]")]
	public class GameserverController : ControllerBase
	{

		private readonly GameserverService _gameserverService;

		public GameserverController(GameserverService gameserverService)
		{
			_gameserverService = gameserverService;
		}

		[HttpPost]
		[Route("verify/{jwt}")]
		public IActionResult VerifyJWT(string jwt)
		{
			return Ok(_gameserverService.VerifyJWT(jwt));
		}

	}
}
