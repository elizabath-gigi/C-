using Microsoft.AspNetCore.Identity.Data;
using OrderManagement.DTOs;
using OrderManagement.Models;

namespace OrderManagement.Interface
{
    public interface IAuthenticationServices
    {
        public Task<List<User>> GetUsers();
        public Task<User> Register(RegisterRequestDto request);
        public Task<string> Login(LoginRequestDto request);
        public Task<string> Reset(ResetRequestDto request);
        //Task<string> Register(RegisterRequestDto request);
        //Task<string> Login(LoginRequestDto request);
    }
}
