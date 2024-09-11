using DK.GenericLibrary;
using DK.GenericLibrary.Interfaces;
using LoginApi.Context;
using LoginApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddTransient<UserService>();
builder.Services.AddTransient(typeof(IAsyncRepository<>), typeof(AsyncRepository<>));
builder.Services.AddDbContextFactory<UserContext>();



var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
	var services = scope.ServiceProvider;
	var context = services.GetRequiredService<UserContext>();

	context.Database.EnsureCreated();
}

	// Configure the HTTP request pipeline.
	
		app.UseSwagger();
		app.UseSwaggerUI();
	

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
