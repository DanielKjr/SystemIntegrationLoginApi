

////using ChatFeatures;

////var receiver = new WebsocketReceiver();
////receiver.Receive(15672);

////var keepReceiving = true;
////while (keepReceiving)
////{
////	Console.WriteLine("Enter 'exit' to stop receiving messages.");
////	var input = Console.ReadLine();
////	if (input == "exit")
////	{
////		keepReceiving = false;
////	}
////}


//using RabbitMQ.Client;
//using RabbitMQ.Client.Events;
//using System.Data.Common;
//using System.Net.WebSockets;
//using System.Text;
//using System.Threading.Channels;



////Start 3 consumers with 2 with Messages.All.# and one with Messages.All.Info.
////var routingKey = args[0];
//var factory = new ConnectionFactory() { HostName = "localhost", Port = 5672 };
//using (var connection = factory.CreateConnection())
//using (var channel = connection.CreateModel())
//{

//	var topicQueueName = "global_chat";
//	channel.ExchangeDeclare(exchange: "global_chat", type: ExchangeType.Topic);
//	channel.QueueBind(queue: topicQueueName, exchange: "topic",
//	   routingKey: "");


//	var consumer = new EventingBasicConsumer(channel);
//	consumer.Received += (model, ea) =>
//	{
//		var body = ea.Body.ToArray();
//		var message = Encoding.UTF8.GetString(body);
//		Console.WriteLine(" [x] Received {0}", message);
//		channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
//		Console.WriteLine(" [x] Done");

//		string replyQueue = ea.BasicProperties.ReplyTo;
//		string correlationId = ea.BasicProperties.CorrelationId;
//		IBasicProperties props = channel.CreateBasicProperties();
//		props.CorrelationId = correlationId;
//		var ansBody = Encoding.UTF8.GetBytes($"answer from message: {correlationId} on queue: {replyQueue}");
//		channel.BasicPublish(exchange: "",
//						 routingKey: replyQueue,
//						 basicProperties: props,
//						 body: ansBody);

//	};
//	channel.BasicConsume(queue: topicQueueName, autoAck: false, consumer: consumer);

//	Console.WriteLine(" Press [enter] to exit.");
//	Console.ReadLine();
//}

// Initialize connection factory with RabbitMQ details
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

		// Declare the queue
		//string queueName = "global_chat";
		//channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

		//// Bind the queue to the exchange with a valid routing key
		//string routingKey = "global_chat"; // This will capture all messages related to "global_chat"
		//channel.QueueBind(queue: queueName, exchange: exchangeName, routingKey: routingKey);

		string queueName = channel.QueueDeclare().QueueName;

		// Bind the queue to the exchange
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


void PrivateChatConsumer(string username)
{
	var factory = new ConnectionFactory() { HostName = "localhost", Port = 5672 };

	using (var connection = factory.CreateConnection())
	using (var channel = connection.CreateModel())
	{
		// Declare the topic exchange
		string exchangeName = "private_chat";
		channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Topic);

		// Declare the queue
		string queueName = "private_chat_" + username;
		channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

		//channel.QueueDeclare(queue: exchangeName, durable: true, exclusive: false, autoDelete: false, arguments: null);

		// Bind the queue to the exchange with a valid routing key
		string routingKey = "private_chat_"+username; // This will capture all messages related to "global_chat"
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
			}
		};

		// Start consuming messages from the queue
		channel.BasicConsume(queue: exchangeName, autoAck: false, consumer: consumer);

		Console.WriteLine(" Press [enter] to exit.");
		Console.ReadLine();
	}
}


//string apiUrl = "http://localhost:7004/api/chat/createQueue/Daniel";

//var httpClient = new HttpClient();

//string jsonData = @"{""userName"":""Daniel""}";

//StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");
//var response = await httpClient.PostAsync(apiUrl, content);

//if(response.IsSuccessStatusCode)
//{
//	Console.WriteLine("Queue created successfully.");
//	PrivateChatConsumer("Daniel");
//}
//else
//{
//	Console.WriteLine("Failed to create queue.");
//}

void PrivateChatConsumer2(string username)
{
	var factory = new ConnectionFactory() { HostName = "localhost", Port = 5672 };

	using (var connection = factory.CreateConnection())
	using (var channel = connection.CreateModel())
	{

		Console.WriteLine("Creating direct channel to User: Daniel...");
		// Declare the topic exchange
		string exchangeName = "private_chat";
		channel.ExchangeDeclare(exchange: exchangeName, type: ExchangeType.Direct);

		// Declare the queue with a unique name based on the username
		string queueName = "private_chat." + username;
		channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: true, arguments: null);

		// Bind the queue to the exchange with the correct routing key
		Console.WriteLine("Binding Channel...");
		string routingKey = "private_chat." + username; // Ensure this matches the published message
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
			}
		};

		// Start consuming messages from the queue
		channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
		Console.WriteLine("Consuming queue...");
		Console.WriteLine("Press [enter] to exit.");
		Console.ReadLine();
	}
}

//async Task CreateAndConsumeQueueAsync(string username)
//{
//	string apiUrl = $"http://localhost:7004/api/chat/createQueue/{username}";

//	using var httpClient = new HttpClient();

//	string jsonData = $@"{{""userName"":""{username}""}}";
//	StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

//	var response = await httpClient.PostAsync(apiUrl, content);

//	if (response.IsSuccessStatusCode)
//	{
//		Console.WriteLine("Queue created successfully.");
//		// Ensure the queue is created and give it a slight delay before consuming
//		await Task.Delay(1000); // Add a small delay to ensure queue creation completes
//		PrivateChatConsumer2(username); // Start consuming the queue
//	}
//	else
//	{
//		Console.WriteLine("Failed to create queue.");
//	}
//}

// Usage example
//PrivateChatConsumer2("Daniel");
GlobalChatConsumer();