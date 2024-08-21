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
        public Task<User> DeleteUser(string UserName,string deletedBy);
        public UserDto GetById(int UserId);
        public UserDto GetByUsername(string UserName);
        public string GetNameHindi(string UserName);
        public string GenerateWebToken(User user);
    }
}
