using ChatAPI.Misc;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Threading.Channels;
using System.Data.Common;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//attempt to create queues on bootup and not every instance
var factory = new ConnectionFactory() { HostName = "rabbitmq", Port = 5672 };

string[] queues = { "global_chat", "private_chat", "party_chat" };
using (var _connection = factory.CreateConnection())
using (var _channel = _connection.CreateModel())
{
	foreach (string queue in queues)
	{
		_channel.QueueDeclare(queue: queue, durable: true, exclusive: false, autoDelete: false, arguments: null);
		var consumer = new EventingBasicConsumer(_channel);

		_channel.BasicConsume(queue: queue, autoAck: false, consumer: consumer);
	}
	//_channel.QueueDeclare(queue: "global_chat", durable: true, exclusive: false, autoDelete: false, arguments: null);
	//var consumer = new EventingBasicConsumer(_channel);
	
	//_channel.BasicConsume(queue: "global_chat", autoAck: false, consumer: consumer);

}

Thread.Sleep(1000);
//	//ConsumeQueue("global_chat");
//	//ConsumeQueue("private_chat");
//	//ConsumeQueue("party_chat");

//	_channel.QueueDeclare(queue: "global_chat", durable: true, exclusive: false, autoDelete: false, arguments: null);
//	var consumer = new EventingBasicConsumer(_channel);
//	_channel.BasicConsume(queue: "global_chat", autoAck: false, consumer: consumer);

//	_channel.QueueDeclare(queue: "private_chat", durable: true, exclusive: false, autoDelete: false, arguments: null);
//	 consumer = new EventingBasicConsumer(_channel);
//	_channel.BasicConsume(queue: "private_chat", autoAck: false, consumer: consumer);

//	_channel.QueueDeclare(queue: "party_chat", durable: true, exclusive: false, autoDelete: false, arguments: null);
//	consumer = new EventingBasicConsumer(_channel);
//	_channel.BasicConsume(queue: "party_chat", autoAck: false, consumer: consumer);
//}

builder.Services.AddTransient<RabbitMQService>();

//builder.Services.AddTransient<RequestDelegate>();

var app = builder.Build();
//app.UseWebSockets();
//app.UseMiddleware<ChatWebSocketMiddleware>();
//app.UseChatWebSocket();
// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
	app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
