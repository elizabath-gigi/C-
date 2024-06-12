using Microsoft.AspNetCore.Identity.Data;
using OrderManagement.DTOs;

namespace OrderManagement.Interface
{
    public interface IAuthenticationServices
    {
        public Task<string> Register(RegisterRequestDto request);
        public Task<string> Login(LoginRequestDto request);

        public Task<string> Reset(ResetRequestDto request);
        //Task<string> Register(RegisterRequestDto request);
        //Task<string> Login(LoginRequestDto request);
    }
}
