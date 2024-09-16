using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;



void GlobalChatConsumer()
{
	var factory = new ConnectionFactory() { HostName = "localhost", Port = 5672 };

	using (var connection = factory.CreateConnection())
	using (var channel = connection.CreateModel())
	{
		// Declare the topic exchange
		string exchangeName = "global_chat";
		channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Fanout);

		string queueName = channel.QueueDeclare().QueueName;

		//bind to global chat with own name
		channel.QueueBind(queue: queueName, exchange: exchangeName, routingKey: "");
		// Setup the consumer to listen to the queue
		var consumer = new EventingBasicConsumer(channel);
		consumer.Received += (model, ea) =>
		{
			var body = ea.Body.ToArray();
			var message = Encoding.UTF8.GetString(body);

			// Display the message on the console
			Console.WriteLine($"Chat [x] Received: {message}");

			// Acknowledge that the message was received and processed
			channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);

			Console.WriteLine("Chat [x] Done");

			// Example reply logic
			string replyQueue = ea.BasicProperties.ReplyTo;
			string correlationId = ea.BasicProperties.CorrelationId;
			if (!string.IsNullOrEmpty(replyQueue))
			{
				IBasicProperties props = channel.CreateBasicProperties();
				props.CorrelationId = correlationId;
				var ansBody = Encoding.UTF8.GetBytes($"Answer from message: {correlationId} on queue: {replyQueue}");
				channel.BasicPublish(exchange: "",
									 routingKey: replyQueue,
									 basicProperties: props,
									 body: ansBody);
			}
		};

		// Start consuming messages from the queue
		channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);

		Console.WriteLine(" Press [enter] to exit.");
		Console.ReadLine();
	}
}

void PrivateChatConsumer2(string username)
{
	var factory = new ConnectionFactory() { HostName = "localhost", Port = 5672 };

	using (var connection = factory.CreateConnection())
	using (var channel = connection.CreateModel())
	{
		// Declare the topic exchange
		string exchangeName = "private_chat";
		channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Direct);

		// create queue for user, will be deleted when user disconnects
		string queueName = "private_chat." + username;
		channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: true, arguments: null);

	
		string routingKey = "private_chat." + username; 
		channel.QueueBind(queue: queueName, exchange: exchangeName, routingKey: routingKey);

		// Setup the consumer to listen to the queue
		var consumer = new EventingBasicConsumer(channel);
		consumer.Received += (model, ea) =>
		{
			var body = ea.Body.ToArray();
			var message = Encoding.UTF8.GetString(body);

			// Display the message on the console
			Console.WriteLine($"Chat [x] Received: {message}");

			// Acknowledge that the message was received and processed
			channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);

			Console.WriteLine("Chat [x] Done");

			// Example reply logic
			string replyQueue = ea.BasicProperties.ReplyTo;
			string correlationId = ea.BasicProperties.CorrelationId;
			if (!string.IsNullOrEmpty(replyQueue))
			{
				IBasicProperties props = channel.CreateBasicProperties();
				props.CorrelationId = correlationId;
				var ansBody = Encoding.UTF8.GetBytes($"Answer from message: {correlationId} on queue: {replyQueue}");
				
				channel.BasicPublish(exchange: "",
									 routingKey: replyQueue,
									 basicProperties: props,
									 body: ansBody);

				channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
			}

		};

		// Start consuming messages from the queue
		channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
		Console.WriteLine("Press [enter] to exit.");
		Console.ReadLine();
	}
}


PrivateChatConsumer2("Daniel");
//GlobalChatConsumer();