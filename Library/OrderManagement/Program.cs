using OrderManagement.Interface;
using OrderManagement.Models;
using OrderManagement.Services;
using Microsoft.EntityFrameworkCore;
//using OrderManagement.Models;
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

app.Run();

