using ChatAPI.Misc;
using Microsoft.AspNetCore.Mvc;

namespace ChatAPI.Controllers
{
	[ApiController]
	[Route("api/chat")]
	public class ChatController(RabbitMQService _rabbitMQService) : ControllerBase
	{

		[HttpGet("{chatType}/history")]
		public IActionResult GetChatHistory(string chatType)
		{
			// Fetch chat history from database or message store
			return Ok(new { messages = new string[] { "Welcome!", "Hello there!" } });
		}


		[HttpPost("{chatType}/send")]
		public IActionResult SendMessage(string chatType, [FromBody] string message)
		{
			// Determine the queue based on the chat type (global, private, party)
			string queueName = GetQueueName(chatType);

			if (queueName == null)
			{
				return BadRequest("Invalid chat type.");
			}

			// Send message to RabbitMQ queue
			_rabbitMQService.SendMessageToQueue(queueName, message);

			return Ok(new { Status = "Message sent", ChatType = chatType, Message = message });
		}

		private string GetQueueName(string chatType)
		{
			switch (chatType.ToLower())
			{
				case "global":
					return "global_chat";
				case "private":
					return "private_chat";
				case "party":
					return "party_chat";
				default:
					return null;
			}
		}
	}

}
