//using GameserverAPI.Misc;
//using Microsoft.AspNetCore.Mvc;

//namespace GameserverAPI.Controllers
//{
//	[Route("api/[controller]")]
//	[ApiController]
//	public class Register : ControllerBase
//	{
//		private IStorage storage;

//		public Register(IStorage storage) { this.storage = storage; }
//		[HttpPost]
//		public async Task<IActionResult> RegisterService([FromBody] ServiceDetails serviceDetails)
//		{
//			bool isHealthy = await PerformHealthCheck(serviceDetails.ServiceUrl);

//			if (isHealthy)
//			{
//				Console.WriteLine($"succesfully registered: {serviceDetails.ServiceName} at url: {serviceDetails.ServiceUrl}");
//				storage.AddItem(serviceDetails.ServiceName, serviceDetails.ServiceUrl);
//				return Ok(new { message = "Service registered successfully." });
//			}

//			Console.WriteLine($"did not register service {serviceDetails.ServiceName} due to unhealthy");
//			return BadRequest(new { message = "Health check failed. Service not registered." });
//		}

//		[HttpGet("GetAllServiceNames")]
//		public IActionResult GetRegisteredServices()
//		{
//			return Ok(storage.GetAllServiceNames());
//		}
//		private async Task<bool> PerformHealthCheck(string serviceUrl)
//		{
//			try
//			{
//				using (var client = new HttpClient())
//				{
//					var response = await client.GetAsync($"{serviceUrl}/hc");
//					return response.IsSuccessStatusCode;
//				}
//			}
//			catch
//			{
//				return false;
//			}
//		}
//	}

//	public class ServiceDetails
//	{
//		public string ServiceName { get; set; }
//		public string ServiceUrl { get; set; }
//	}
//}
