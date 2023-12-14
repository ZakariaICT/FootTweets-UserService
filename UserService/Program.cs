using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using UserService.Data;
using UserService.Repositories;
using UserService.AsyncDataServices;

using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();

builder.Services.AddControllers();

// Configuration setup
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
var configuration = builder.Configuration;


builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(configuration.GetConnectionString("PostgresConnection")));


builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "User API", Version = "v1" });
});

builder.Services.AddScoped<IUserRepo, UserRepo>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
