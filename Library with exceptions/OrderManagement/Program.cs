using OrderManagement.Interface;
using OrderManagement.Models;
using OrderManagement.Services;
using Microsoft.EntityFrameworkCore;
using log4net;
using log4net.Config;
//using OrderManagement.Models;
[assembly: log4net.Config.XmlConfigurator(ConfigFile = "log4net.config", Watch = true)]
//log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo("log4net.config"));

var logRepository = LogManager.GetRepository(System.Reflection.Assembly.GetEntryAssembly());
XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

ILog log = LogManager.GetLogger(typeof(Program));
log.Info("Application started.");

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IserviceClass,ServiceClass>();
builder.Services.AddDbContext<LibraryContext>(options => 
options.UseSqlServer(builder.Configuration.GetConnectionString("dbconn")));
var app = builder.Build();
//public void ConfigureServices(IServiceCollection services)
//{
//    // Other service registrations...

//    services.AddScoped<ServiceClass>(); // Assuming LibraryService is a scoped service
//}
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

try
{
    app.Run();
}
finally
{
    log.Info("Application ended.");
}



