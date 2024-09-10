using GameserverAPI.Services;
using Microsoft.AspNetCore.Mvc;

namespace GameserverAPI.Controllers
{
	[Route("api/v1/[controller]")]
	public class GameserverController : ControllerBase
	{

		private readonly GameserverService _gameserverService;

		public GameserverController(GameserverService gameserverService)
		{
			_gameserverService = gameserverService;
		}

		

	}
}
