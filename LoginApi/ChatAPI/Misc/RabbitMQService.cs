using ChatAPI.Controllers;
using Microsoft.AspNetCore.Connections;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace ChatAPI.Misc
{
	public class RabbitMQService
	{
		//private IConnection _connection;
		//private IModel _channel;

		public void SendGlobalMessage(string queueName, string message)
		{	
			//Usings instead of reference because if left as transient it will create a 
			//new connection every time, BUT also keep it alive for some reason.
			//So this disposes it immediately
			var factory = new ConnectionFactory() { HostName = "rabbitmq", Port = 5672 };
			using (var _connection = factory.CreateConnection())
			using (var _channel = _connection.CreateModel())
			{
				// Convert the message to a byte array
				var body = Encoding.UTF8.GetBytes(message);

				// Send the message to the queue
				//if queuename is not global_chat, make it empty as private has no exchange
				_channel.BasicPublish(exchange: queueName == "global_chat" ? queueName : "",
									  routingKey: queueName,
									  basicProperties: null,
									  body: body);

				Console.WriteLine($" [x] Sent {message}");
			}

		}


		public RabbitMQService()
		{
			//name from the compose file 
			//var factory = new ConnectionFactory() { HostName = "rabbitmq", Port = 5672 };
			//_connection = factory.CreateConnection();
			//_channel = _connection.CreateModel();

			// Declare and consume messages from different chat queues

			//ConsumeQueue("global_chat");
			//ConsumeQueue("private_chat");
			//ConsumeQueue("party_chat");
		}


		//private void ConsumeQueue(string queueName)
		//{
		//	_channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);
		//	var consumer = new EventingBasicConsumer(_channel);
		//	//consumer.Received += (model, ea) =>
		//	//{
		//	//	var body = ea.Body.ToArray();
		//	//	var message = Encoding.UTF8.GetString(body);

		//	//	// Broadcast message to WebSocket clients
		//	//	//BroadcastToWebSocketClients(message);
		//	//};

		//	_channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
		//}

		//public void SendPrivateMessage(string queueName, string message)
		//{

		//	var factory = new ConnectionFactory() { HostName = "rabbitmq", Port = 5672 };
		//	using (var _connection = factory.CreateConnection())
		//	using (var _channel = _connection.CreateModel())
		//	{
		//		var body = Encoding.UTF8.GetBytes(message);

		//		_channel.BasicPublish(exchange: "",
		//							  routingKey: queueName,
		//							  basicProperties: null,
		//							  body: body);

		//		Console.WriteLine($" [x] Sent {message}");
		//	}

		//}

		////TODO opdater properties til at matche hvad end party skal kunne
		//public void SendPartyMessage(string queueName, string message)
		//{

		//	var body = Encoding.UTF8.GetBytes(message);

		//	_channel.BasicPublish(exchange: "",
		//						  routingKey: queueName,
		//						  basicProperties: null,
		//						  body: body);

		//	Console.WriteLine($" [x] Sent {message}");
		//}
	
		//private void BroadcastToWebSocketClients(string message)
		//{
		//	// Implementation to send message via WebSocket
		//	// Can use ChatWebSocketMiddleware for broadcasting to connected clients
		//	ChatWebSocketMiddleware.Broadcast(message).Wait();
		//}



		//public void Dispose()
		//{
		//	_channel.Close();
		//	_connection.Close();
		//}
	}

}
