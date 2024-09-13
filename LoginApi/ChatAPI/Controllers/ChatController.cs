﻿using ChatAPI.Misc;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

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


		[HttpPost("sendGlobal")]
		public IActionResult SendGlobalMessage([FromBody] string message)
		{
			// Determine the queue based on the chat type (global, private, party)
			

			// Send message to RabbitMQ queue
			_rabbitMQService.SendGlobalMessage("global_chat", message);

			return Ok(new { Status = "Message sent", ChatType = "global_chat", Message = message });
		}


		[HttpPost("sendPrivate/{targetUser}")]
		public IActionResult SendPrivateMessage(string targetUser, [FromBody] string message)
		{
			//Sends a private message to a specific user
			string queueName = "private_chat."+targetUser;
			_rabbitMQService.SendGlobalMessage(queueName, message);

			return Ok(new { Status = "Message sent", ChatType = queueName, Message = message });
		}


		[HttpPost("sendParty/{partyId}")]
		public IActionResult SendPartyMessage(string partyId, [FromBody] string message)
		{
			string queueName = "party_chat." + partyId;

			// Send message to RabbitMQ queue
			_rabbitMQService.SendGlobalMessage(queueName, message);

			return Ok(new { Status = "Message sent", ChatType = queueName, Message = message });
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
