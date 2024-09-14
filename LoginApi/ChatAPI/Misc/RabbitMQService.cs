using ChatAPI.Controllers;
using Microsoft.AspNetCore.Connections;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace ChatAPI.Misc
{
	public class RabbitMQService
	{
	

		public void SendMessage(string queueName, string message)
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


	}

}
