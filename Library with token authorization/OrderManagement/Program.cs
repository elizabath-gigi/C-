using OrderManagement.Interface;
using OrderManagement.Models;
using OrderManagement.Services;
using Microsoft.EntityFrameworkCore;
using log4net;
using log4net.Config;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;


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

var Configuration = new ConfigurationBuilder()
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .Build();


builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(swagger =>
{
//This is to generate the Default UI of Swagger Documentation  
swagger.SwaggerDoc("v1", new OpenApiInfo
{
    Version = "v1",
    Title = "JWT Token Authentication API",
    Description = "ASP.NET Core 3.1 Web API"
});
// To Enable authorization using Swagger (JWT)  
swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
{
    Name = "Authorization",
    Type = SecuritySchemeType.ApiKey,
    Scheme = "Bearer",
    BearerFormat = "JWT",
    In = ParameterLocation.Header,
    Description = "JWT token for Authorization. Example: Bearer 'YOUR_JWT_TOKEN'",
}
);
    swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                          new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            new string[] {}

                    }
                });
});

builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = false,
        ValidateIssuerSigningKey = true,
        ValidIssuer = Configuration["Jwt:Issuer"],
        ValidAudience = Configuration["Jwt:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"])) //Configuration["JwtToken:SecretKey"]  
    };
});

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






