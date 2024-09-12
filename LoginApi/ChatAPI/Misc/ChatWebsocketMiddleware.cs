using System.Net.WebSockets;
using System.Text;

namespace ChatAPI.Misc
{
	public class ChatWebSocketMiddleware
	{
		private readonly RequestDelegate _next;
		private static List<WebSocket> _sockets = new List<WebSocket>();

		public ChatWebSocketMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			if (context.WebSockets.IsWebSocketRequest)
			{
				var socket = await context.WebSockets.AcceptWebSocketAsync();
				_sockets.Add(socket);

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

			while (!result.CloseStatus.HasValue)
			{
				var message = Encoding.UTF8.GetString(buffer, 0, result.Count);

				// Broadcast the message to all connected WebSocket clients
				foreach (var s in _sockets)
				{
					if (s.State == WebSocketState.Open)
					{
						await s.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);
					}
				}

				result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
			}

			_sockets.Remove(socket);
			await socket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
		}
	}

}
