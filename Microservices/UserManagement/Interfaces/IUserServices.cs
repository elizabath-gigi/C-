using UserManagement.DTOs;
using UserManagement.Models;

namespace UserManagement.Interfaces
{
    public interface IUserServices
    {
        public Task<List<User>> GetUsers();
        public Task<User> Register(RegisterRequestDto request);
        public Task<User> Login(LoginRequestDto request);
        public Task<string> Reset(ResetRequestDto request);
        public User GetById(int id);
        public string GenerateWebToken(User user);
    }
}
