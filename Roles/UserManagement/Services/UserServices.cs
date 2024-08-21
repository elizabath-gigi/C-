using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using UserManagement.Models;
using UserManagement.DTOs;
using UserManagement.Interfaces;
using UserManagement.Exceptions;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Configuration;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.Extensions.Options;
using static UserManagement.Interfaces.IUserServices;
using UserManagement.DTOs;
using UserManagement.Models;
    

namespace OrderManagement.Services
{
    public class UserServices : IUserServices
    {
        private readonly LibraryManagementContext _context;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(UserServices));
        private IConfiguration _config;


        public UserServices(LibraryManagementContext context, IConfiguration configuration)
        {
            _context = context;
            _config = configuration;


        }
        public async Task<List<User>> GetUsers()
        {
            var user = await _context.Users.Where(x => x.IsDeleted == 0).ToListAsync();
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
            ValidateUsername(request.UserName);
            ValidatePassword(request.Password);
            var userByEmail = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            var userByUserName = await _context.Users.FirstOrDefaultAsync(u => u.UserName == request.UserName && u.IsDeleted == 0);


            if (userByEmail != null || userByUserName != null)
            {
                log.Debug($"User with email {request.Email} or username {request.UserName} already exists.");
                throw new ArgumentsException($"Registration failed.");
            }

            var user = new User
            {

                Email = request.Email,
                UserName = request.UserName,
                Password = HashPassword(request.Password),
                Role = "User",
                //NameHindi=request.NameHindi
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            log.Info($"User with email {request.Email} or username {request.UserName} registered successfully.");
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
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == request.UserName || u.Email == request.UserName && u.IsDeleted == 0);

            if (user == null || !VerifyPassword(request.Password, user.Password))
            {
                log.Debug($"Unable to authenticate user {request.UserName}");
                throw new ArgumentsException($"Login Failed.");
            }


            log.Info($"User with username {request.UserName} login successfully.");

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
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == request.UserName || u.Email == request.UserName && u.IsDeleted == 0);

            if (user == null || !VerifyPassword(request.OldPassword, user.Password))
            {
                log.Debug($"Unable to authenticate user {request.UserName}");
                throw new ArgumentsException($"Password Reset Failed.");
            }
            user.Password = HashPassword(request.NewPassword);
            _context.SaveChanges();
            log.Info($"User with username {request.UserName} reset password successfully.");
            return "Password Reset Successfully";
        }
        /// <summary>
        /// To Hash the password before storing it in the DB
        /// </summary>
        /// <param name="password"></param>
        /// <returns>string</returns>
        public string HashPassword(string password)
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
        private void ValidateUsername(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName) || userName.Length < 3)
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
            Regex exp = new Regex(@"^(?=.*[A-Z])(?=.*[^a-zA-Z0-9]).{6,}$");
            if (!exp.IsMatch(password))
            {
                throw new ArgumentsException("Password must be at least 6 characters long and must have at least one special character and uppercase letter.");
            }

        }

        /// <summary>
        /// To generate JWT Token
        /// </summary>
        /// <param name="user"></param>
        /// <returns>JwtSecurityToken</returns>

        public UserDto GetById(int UserId)
        {
            User user = _context.Users.FirstOrDefault(x => x.UserId == UserId);
            if (user == null || user.IsDeleted == 1)
            {
                log.Debug("The user is not found");
                throw new IdNotFoundException("The user doesn't exist");
            }
            UserDto userDto = new UserDto
            {
                UserId = user.UserId,
                UserName = user.UserName,
                Email = user.Email,
                Role=user.Role                
            };
            return userDto;
        }
       
        public UserDto GetByUsername(string UserName)
        {
            User user = _context.Users.FirstOrDefault(x => x.UserName == UserName);
            if (user == null || user.IsDeleted == 1)
            {
                log.Debug("The user is not found");
                throw new IdNotFoundException("The user doesn't exist");
            }
            UserDto userDto = new UserDto
            {
                UserId = user.UserId,
                UserName = user.UserName,
                Email = user.Email,
                Role = user.Role
            };
            return userDto;
        }
        public string GetNameHindi(string UserName)
        {
            User user = _context.Users.FirstOrDefault(x => x.UserName == UserName);
            if (user == null ||user.IsDeleted==1)
            {
                log.Debug("The user is not found");
                throw new IdNotFoundException("The user doesn't exist");
            }
            return user.NameHindi;
        }
        public async Task<User> DeleteUser(string UserName,string deletedBy)
        {
            
            var user = _context.Users.FirstOrDefault(x => x.UserName == UserName);

            
            if (user == null||user.IsDeleted==1)
            {
                log.Debug("The user is not found or is already marked as deleted, delete failed");
                throw new IdNotFoundException("The user doesn't exist or is already deleted.");
            }
            else if(user.Role=="Admin")
            {
                log.Debug("The user found is an Admin, delete failed");
                throw new IdNotFoundException("The user found is an Admin, delete failed.");
            }
            user.IsDeleted = 1; 
            user.DeletedBy = deletedBy;
            user.DeletedOn= DateTime.Now;
            await _context.SaveChangesAsync();

            log.Info("The user details have been marked as deleted successfully in DB");
            return user;
        }


        public string GenerateWebToken(User user)
        {
            // generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", user.UserId.ToString()),
                                                     new Claim("username",user.UserName),
                                                     new Claim(ClaimTypes.Role, user.Role)}),
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }

}

