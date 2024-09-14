using ChatAPI.Misc;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Threading.Channels;
using System.Data.Common;
using Polly;
using System;
using RabbitMQ.Client.Exceptions;
using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//the rabbitMQ needs to be running before the ChatAPI can connect, so we need to retry the connection
//in case the rabbitMQ is booted up faster
var policy = Policy
	.Handle<BrokerUnreachableException>()  
	.Or<SocketException>()                
	.WaitAndRetryAsync(new[]
	{
		TimeSpan.FromSeconds(1),
		TimeSpan.FromSeconds(3),
		TimeSpan.FromSeconds(6),
		TimeSpan.FromSeconds(10),
		TimeSpan.FromSeconds(20),
		TimeSpan.FromSeconds(30)
	},
	(exception, timespan, retryCount, context) =>
	{
		Console.WriteLine($"Attempt {retryCount} failed: {exception.Message}. Retrying in {timespan.TotalSeconds} seconds.");
	});


await policy.ExecuteAsync(async () =>
{

	var factory = new ConnectionFactory() { HostName = "rabbitmq", Port = 5672 };

	string[] queues = { "global_chat", "private_chat", "party_chat" };
	using (var _connection = await Task.Run(() => factory.CreateConnection()))
	using (var _channel = _connection.CreateModel())
	{
		foreach (string queue in queues)
		{
			_channel.QueueDeclare(queue: queue, durable: true, exclusive: false, autoDelete: false, arguments: null);
			var consumer = new EventingBasicConsumer(_channel);

			_channel.BasicConsume(queue: queue, autoAck: false, consumer: consumer);
		}


	}

});
builder.Services.AddTransient<RabbitMQService>();



var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();


app.UseAuthorization();

app.MapControllers();


app.Run();
