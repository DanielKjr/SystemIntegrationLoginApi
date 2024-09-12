using Microsoft.AspNetCore.Connections;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace ChatAPI.Misc
{
	public class RabbitMQService
	{
		private IConnection _connection;
		private IModel _channel;

		public RabbitMQService()
		{
			//name from the compose file 
			var factory = new ConnectionFactory() { HostName = "rabbitmq", Port = 5672 };
			_connection = factory.CreateConnection();
			_channel = _connection.CreateModel();

			// Declare and consume messages from different chat queues
			ConsumeQueue("global_chat");
			ConsumeQueue("private_chat");
			ConsumeQueue("party_chat");
		}

		private void ConsumeQueue(string queueName)
		{
			_channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
			var consumer = new EventingBasicConsumer(_channel);
			consumer.Received += (model, ea) =>
			{
				var body = ea.Body.ToArray();
				var message = Encoding.UTF8.GetString(body);

				// Broadcast message to WebSocket clients
				BroadcastToWebSocketClients(message);
			};

			_channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
		}

		public void SendMessageToQueue(string queueName, string message)
		{
			// Declare the queue (if it doesn't exist yet)
			_channel.QueueDeclare(queue: queueName,
								  durable: true,
								  exclusive: false,
								  autoDelete: false,
								  arguments: null);

			// Convert the message to a byte array
			var body = Encoding.UTF8.GetBytes(message);

			// Send the message to the queue
			_channel.BasicPublish(exchange: "",
								  routingKey: queueName,
								  basicProperties: null,
								  body: body);

			Console.WriteLine($" [x] Sent {message}");
		}

		private void BroadcastToWebSocketClients(string message)
		{
			// Implementation to send message via WebSocket
			// Can use ChatWebSocketMiddleware for broadcasting to connected clients
		}

		public void Dispose()
		{
			_channel.Close();
			_connection.Close();
		}
	}

}
