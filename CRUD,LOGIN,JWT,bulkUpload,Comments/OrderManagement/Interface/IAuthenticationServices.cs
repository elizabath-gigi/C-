using Microsoft.AspNetCore.Identity.Data;
using OrderManagement.DTOs;
using OrderManagement.Models;
using System.IdentityModel.Tokens.Jwt;

namespace OrderManagement.Interface
{
    public interface IAuthenticationServices
    {
        public Task<List<User>> GetUsers();
        public Task<User> Register(RegisterRequestDto request);
        public Task<User> Login(LoginRequestDto request);
        public Task<string> Reset(ResetRequestDto request);
        public JwtSecurityToken GenerateJSONWebToken(User user);
        
    }
}
