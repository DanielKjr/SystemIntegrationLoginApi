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
		public Task<string> VerifyJWT(string jwt)
		{
			return _gameserverService.VerifyJWT(jwt);
		}

	}
}
