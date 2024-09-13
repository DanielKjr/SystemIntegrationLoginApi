using System.Net.WebSockets;
using System.Text;

namespace ChatAPI.Misc
{
	//public static class RequestWebSocketMiddleware
	//{
	//	public static IApplicationBuilder UseChatWebSocket(this IApplicationBuilder builder)
	//	{
	//		return builder.UseMiddleware<ChatWebSocketMiddleware>();
	//	}
	//}
	public class ChatWebSocketMiddleware
	{
		private readonly RequestDelegate _next;
		private static List<WebSocket> _clients = new List<WebSocket>();

		public ChatWebSocketMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			if (context.WebSockets.IsWebSocketRequest)
			{
				var socket = await context.WebSockets.AcceptWebSocketAsync();
				_clients.Add(socket);

				// Handle WebSocket communication
				await ReceiveMessagesAsync(socket);
			}
			else
			{
				await _next(context);
			}
		}

		private async Task ReceiveMessagesAsync(WebSocket socket)
		{
			var buffer = new byte[1024 * 4];
			var result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
			Console.WriteLine("Recieving");
			while (!result.CloseStatus.HasValue)
			{
				var message = Encoding.UTF8.GetString(buffer, 0, result.Count);

				// Broadcast the message to all connected WebSocket clients
				foreach (var s in _clients)
				{
					if (s.State == WebSocketState.Open)
					{
						await s.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);
					}
				}

				result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
			}

			_clients.Remove(socket);
			await socket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
		}

		public static async Task Broadcast(string message)
		{
			foreach (var client in _clients.ToList())
			{
				if (client.State == WebSocketState.Open)
				{
					var buffer = Encoding.UTF8.GetBytes(message);
					await client.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
				}
			}
		}
	}

}
