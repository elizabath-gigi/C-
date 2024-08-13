using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using LibraryManagement.Interfaces;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure;
using System.Net.Http;
using System.Text.Json;
using LibraryManagement.DTOs;
//using WebApi.Services;

namespace OrderManagement.Middleware
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private IConfiguration _config;
        private readonly HttpClient _httpClient;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(JwtMiddleware));
        public JwtMiddleware(RequestDelegate next, IConfiguration configuration, HttpClient httpClient)
        {
            _next = next;
            _config = configuration;
            _httpClient = httpClient;
        }

        public async Task Invoke(HttpContext context)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
                await attachUserToContext(context, token);

            await _next(context);
        }

        private async Task  attachUserToContext(HttpContext context, string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"]);
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);
                /*var response= await _httpClient.GetAsync($"https://localhost:7261/User/getUserById?UserId={userId}\r\n");
                var userJson = await response.Content.ReadAsStringAsync();
                var user = JsonSerializer.Deserialize<User>(userJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });*/
        
                // attach user to context on successful jwt validation
                context.Items["UserId"] =userId;
            }
            catch
            {
                log.Info("Some issue with middleware");
            }
        }
    }
}