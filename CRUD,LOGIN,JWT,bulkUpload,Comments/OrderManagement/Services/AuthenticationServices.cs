using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using OrderManagement.Models;
using OrderManagement.DTOs;
using OrderManagement.Interface;
using OrderManagement.Exceptions;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Configuration;
using System.Text.RegularExpressions;


namespace OrderManagement.Services
{
    public class AuthenticationServices : IAuthenticationServices
    {
        private readonly LibraryContext _context;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(LibraryServices));
        private IConfiguration _config;
        public AuthenticationServices(LibraryContext context, IConfiguration configuration)
        {
            _context = context;
            _config = configuration;

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


        /// <summary>
        /// Register a new user to the DB
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Task<User></returns>
        /// <exception cref="ArgumentsException"></exception>
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
        /// <summary>
        /// Login with valid credentials
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Task<User></returns>
        /// <exception cref="ArgumentsException"></exception>
        public async Task<User> Login(LoginRequestDto request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == request.Username || u.Email == request.Username);

            if (user == null || !VerifyPassword(request.Password, user.Password))
            {
                log.Debug($"Unable to authenticate user {request.Username}");
                throw new ArgumentsException($"Login Failed.");
            }
          

            log.Info($"User with username {request.Username} login successfully.");
            
            return user;
        }

        /// <summary>
        /// Reset the password
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Task<string></returns>
        /// <exception cref="ArgumentsException"></exception>
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
        /// <summary>
        /// To Hash the password before storing it in the DB
        /// </summary>
        /// <param name="password"></param>
        /// <returns>string</returns>
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
        /// <summary>
        /// To check if the storedHash in the DB and the input password is the same
        /// </summary>
        /// <param name="inputPassword"></param>
        /// <param name="storedHash"></param>
        /// <returns></returns>
        private bool VerifyPassword(string inputPassword, string storedHash)
        {
            var hashOfInput = HashPassword(inputPassword);
            return hashOfInput == storedHash;
        }
        /// <summary>
        /// To check if the email is in the correct format
        /// </summary>
        /// <param name="email"></param>
        /// <exception cref="ArgumentsException"></exception>
        private void ValidateEmail(string email)
        {
            var emailAttribute = new EmailAddressAttribute();
            if (!emailAttribute.IsValid(email))
            {
                throw new ArgumentsException("Invalid email format.");
            }
        }
        /// <summary>
        /// To check if username has atleast 3 characters
        /// </summary>
        /// <param name="username"></param>
        /// <exception cref="ArgumentsException"></exception>
        private void ValidateUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username) || username.Length < 3)
            {
                throw new ArgumentsException("Username must be at least 3 characters long.");
            }
        }
        /// <summary>
        /// To check if the password has at least 6 characters, have at least one special character and an uppercase letter
        /// </summary>
        /// <param name="password"></param>
        /// <exception cref="ArgumentsException"></exception>
        private void ValidatePassword(string password)
        {
            Regex exp = new Regex(@"^(?=.[A-Z])(?=.[^a-zA-Z0-9]).{6,}$");
            if (!exp.IsMatch(password))
            {
                throw new ArgumentsException("Password must be at least 6 characters long, must have at least one special character and an uppercase letter.");
            }
        }

        /// <summary>
        /// to generate JWT Token
        /// 
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns>JwtSecurityToken</returns>
        public JwtSecurityToken GenerateJSONWebToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
        new Claim(JwtRegisteredClaimNames.Sub, user.Username),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Issuer"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials);

            return token;
        }


    }
}
