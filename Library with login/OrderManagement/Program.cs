using OrderManagement.Interface;
using OrderManagement.Models;
using OrderManagement.Services;
using Microsoft.EntityFrameworkCore;
using log4net;
using log4net.Config;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection;

[assembly: XmlConfigurator(ConfigFile = "log4net.config", Watch = true)]


var logRepository = LogManager.GetRepository(System.Reflection.Assembly.GetEntryAssembly());
XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

ILog log = LogManager.GetLogger(typeof(Program));
log.Info("Application started.");

var builder = WebApplication.CreateBuilder(args);



builder.Services.AddDbContext<LibraryContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<ILibraryServices, LibraryServices>();
builder.Services.AddScoped<IAuthenticationServices, AuthenticationServices>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options =>
        {
            options.LoginPath = "/User/Login"; // Redirect to login if not authenticated
            options.AccessDeniedPath = "/User/AccessDenied"; // Redirect if access denied
        });



builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IAuthenticationServices, AuthenticationServices>();
builder.Services.AddDistributedMemoryCache();


var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();






