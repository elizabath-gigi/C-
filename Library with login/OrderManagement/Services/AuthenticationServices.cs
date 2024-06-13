using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore;
using OrderManagement.Models;
using OrderManagement.DTOs;
using OrderManagement.Interface;
using OrderManagement.Exceptions;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace OrderManagement.Services
{
    public class AuthenticationServices : IAuthenticationServices
    {
        private readonly LibraryContext _context;
        private readonly IConfiguration _configuration;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(LibraryServices));

        public AuthenticationServices(LibraryContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        public async Task<List<User>> GetUsers()
        {
            var user = await _context.Users.ToListAsync();
            if (user.Count == 0)
            {
                log.Debug("The user DB is null");
            }
            log.Info("The contents of the User DB is retrieved");
            return user;

        }



        public async Task<User> Register(RegisterRequestDto request)
        {
            ValidateEmail(request.Email);
            ValidateUsername(request.Username);
            ValidatePassword(request.Password);
            var userByEmail = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            var userByUsername = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);

            if (userByEmail != null || userByUsername != null)
            {
                log.Debug($"User with email {request.Email} or username {request.Username} already exists.");
                throw new ArgumentsException($"Registration failed.");
            }

            var user = new User
            {
             
                Email = request.Email,
                Username = request.Username,
                Password = HashPassword(request.Password)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            log.Info($"User with email {request.Email} or username {request.Username} registered successfully.");
            return user;
        }

        public async Task<string> Login(LoginRequestDto request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username || u.Email == request.Username);

            if (user == null || !VerifyPassword(request.Password, user.Password))
            {
                log.Debug($"Unable to authenticate user {request.Username}");
                throw new ArgumentsException($"Login Failed.");
                
            }
            log.Info($"User with username {request.Username} login successfully.");
            return "Login Successful";
        }


        public async Task<string> Reset(ResetRequestDto request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username || u.Email == request.Username);

            if (user == null || !VerifyPassword(request.OldPassword, user.Password))
            {
                log.Debug($"Unable to authenticate user {request.Username}");
                throw new ArgumentsException($"Password Reset Failed.");
            }
            user.Password = HashPassword(request.NewPassword);
            _context.SaveChanges();
            log.Info($"User with username {request.Username} reset password successfully.");
            return "Password Reset Successfully";
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
                throw new ArgumentsException("Invalid email format.");
            }
        }

        private void ValidateUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username) || username.Length < 3)
            {
                throw new ArgumentsException("Username must be at least 3 characters long.");
            }
        }

        private void ValidatePassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
            {
                throw new ArgumentsException("Password must be at least 6 characters long.");
            }

            if (!password.Any(char.IsUpper))
            {
                throw new ArgumentsException("Password must contain at least one uppercase letter.");
            }

            if (!password.Any(ch => !char.IsLetterOrDigit(ch)))
            {
                throw new ArgumentsException("Password must contain at least one special character.");
            }
        }

    }
}
