using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using OrderManagement.Models;
using OrderManagement.DTOs;
using OrderManagement.Interface;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Services
{
    public class AuthenticationServices : IAuthenticationServices
    {
        private readonly LibraryContext _context;
        private readonly IConfiguration _configuration;

        public AuthenticationServices(LibraryContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<string> Register(RegisterRequestDto request)
        {
            ValidateEmail(request.Email);
            ValidateUsername(request.Username);
            ValidatePassword(request.Password);
            var userByEmail = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            var userByUsername = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);

            if (userByEmail != null || userByUsername != null)
            {
                throw new ArgumentException($"User with email {request.Email} or username {request.Username} already exists.");
            }

            var user = new User
            {
                //Id = Guid.NewGuid().ToString(),
                Email = request.Email,
                Username = request.Username,
                Password = HashPassword(request.Password)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return "Registration successful";
        }

        public async Task<string> Login(LoginRequestDto request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username || u.Email == request.Username);

            if (user == null || !VerifyPassword(request.Password, user.Password))
            {
                throw new ArgumentException($"Unable to authenticate user {request.Username}");
            }

            return "Login successful";
        }


        // Inside AuthenticationServices class
        public async Task<string> Reset(ResetRequestDto request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username || u.Email == request.Username);

            if (user == null || !VerifyPassword(request.OldPassword, user.Password))
            {
                throw new ArgumentException($"Unable to authenticate user {request.Username}");
            }
            user.Password = HashPassword(request.NewPassword);
            _context.SaveChanges();
            return "Password reset";
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            var builder = new StringBuilder();
            foreach (var b in bytes)
            {
                builder.Append(b.ToString("x2"));
            }
            return builder.ToString();
        }

        private bool VerifyPassword(string inputPassword, string storedHash)
        {
            var hashOfInput = HashPassword(inputPassword);
            return hashOfInput == storedHash;
        }
        private void ValidateEmail(string email)
        {
            var emailAttribute = new EmailAddressAttribute();
            if (!emailAttribute.IsValid(email))
            {
                throw new ArgumentException("Invalid email format");
            }
        }

        private void ValidateUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username) || username.Length < 3)
            {
                throw new ArgumentException("Username must be at least 3 characters long");
            }
        }

        private void ValidatePassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
            {
                throw new ArgumentException("Password must be at least 6 characters long");
            }

        }
    }
}
