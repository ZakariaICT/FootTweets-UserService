using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using UserService.Data;
using UserService.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using System;
using System.Text;
using UserService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Services.AddControllers();

// Configuration setup
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
var configuration = builder.Configuration;

builder.Services.AddDbContext<AppDbContext>();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "User API", Version = "v1" });
});

builder.Services.AddScoped<IUserRepo, UserRepo>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add RabbitMQ service
builder.Services.AddSingleton(serviceProvider =>
{
    var factory = new ConnectionFactory
    {
        Uri = new Uri(configuration["RabbitMQConnection"]),
    };

    var connection = factory.CreateConnection();
    var channel = connection.CreateModel();

    // Declare a queue for user registration requests
    //channel.QueueDeclare("user_registration_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);

    // Declare a queue for UID responses
    channel.QueueDeclare("uid_queue", durable: false, exclusive: false, autoDelete: false, arguments: null);

    // Declare a queue for Deletion responses
    channel.QueueDeclare("user.deleted", durable: false, exclusive: false, autoDelete: false, arguments: null);

    // Set up RabbitMQ consumer
    var rabbitMQConsumer = new RabbitMQConsumer(channel);
    rabbitMQConsumer.StartListening("uid_queue");

    return new RabbitMQService(channel);
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    using var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();
}



// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();


