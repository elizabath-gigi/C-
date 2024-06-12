using OrderManagement.Interface;
using OrderManagement.Models;
using OrderManagement.Services;
using Microsoft.EntityFrameworkCore;
using log4net;
using log4net.Config;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection;
//using OrderManagement.Models;
[assembly: XmlConfigurator(ConfigFile = "log4net.config", Watch = true)]
//log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo("log4net.config"));

var logRepository = LogManager.GetRepository(System.Reflection.Assembly.GetEntryAssembly());
XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

ILog log = LogManager.GetLogger(typeof(Program));
log.Info("Application started.");

var builder = WebApplication.CreateBuilder(args);


// Add and configure authentication
/*builder.Services.AddAuthentication(CookiesAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/User/Login"; // Path to redirect to login if not authenticated
    });*/

// Add services to the container.
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
/*builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});*/


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IAuthenticationServices, AuthenticationServices>();
builder.Services.AddDistributedMemoryCache();

//public void ConfigureServices(IServiceCollection services)
//{
//    // Other service registrations...

//    services.AddScoped<ServiceClass>(); // Assuming LibraryService is a scoped service
//}
// Configure the HTTP request pipeline.
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





