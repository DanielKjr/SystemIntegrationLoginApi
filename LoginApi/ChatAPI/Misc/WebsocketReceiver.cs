using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;

namespace ChatAPI.Misc
{
	public class WebsocketReceiver
	{
		private ClientWebSocket _webSocket;

	

		public async void Receive(int port)
		{
			_webSocket = new ClientWebSocket();
			await _webSocket.ConnectAsync(new System.Uri($"ws://rabbitmq:{port}/global_chat"), CancellationToken.None);

			Console.WriteLine("Connected to WebSocket");

			// Sending a message
			var message = Encoding.UTF8.GetBytes("Hello from reciever");
			await _webSocket.SendAsync(new ArraySegment<byte>(message), WebSocketMessageType.Text, true, CancellationToken.None);

			// Receiving messages
			var buffer = new byte[1024 * 4];
			while (_webSocket.State == WebSocketState.Open)
			{
				var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
				var receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);
				Console.WriteLine($"Message received: {receivedMessage}");
			}
		}
	}
}
