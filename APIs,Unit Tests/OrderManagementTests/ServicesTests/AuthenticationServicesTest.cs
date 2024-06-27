using Azure.Core;
using log4net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using OrderManagement.DTOs;
using OrderManagement.Exceptions;
using OrderManagement.Interface;
using OrderManagement.Models;
using OrderManagement.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagementTests.ServicesTests
{
    public class AuthenticationServicesTest
    {
        private AuthenticationServices _authenticationServices;
        private Mock<IConfiguration> _mockConfig;
        private List<User> sampleUsers  = new List<User>
        {
            new User { Id = 1, Email = "email1@gmail.com" , Username ="User1" , Password = "Password1@123"},
            new User { Id = 2, Email = "email2@gmail.com" , Username ="User2" , Password = "Password2@123"},
            new User { Id = 3, Email = "email3@gmail.com" , Username ="User3" , Password = "Password3@123"}

        };
        public AuthenticationServicesTest()
        {
            _mockConfig = new Mock<IConfiguration>();
        }
        private async Task<LibraryContext> GetMemoryDatabaseContext(int count)
        {
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .BuildServiceProvider();

            var optionsBuilder = new DbContextOptionsBuilder<LibraryContext>()
                .UseInMemoryDatabase(databaseName: "Library")
                .UseInternalServiceProvider(serviceProvider);
            var _context = new LibraryContext(optionsBuilder.Options);
            // context.Database.EnsureCreated();
            if (await _context.Users.CountAsync() <= 0)
            {
                for (int i = 0; i < count; i++)
                {
                    _context.Users.Add(sampleUsers[i]);
                    await _context.SaveChangesAsync();
                }
            }

            return _context;
        }
        private  async Task<LibraryContext> HashPasswordsInTheContext(LibraryContext _context)
        {
            for (int i = 0; i < 3; i++)
            {
                sampleUsers[i].Password=_authenticationServices.HashPassword(sampleUsers[i].Password);
                await _context.SaveChangesAsync();
            }



            return _context;
        }
        [Fact]
        public async void GetUsers_ShouldReturnListOfUsers()
        {
            var _context = await GetMemoryDatabaseContext(3);
            _authenticationServices = new AuthenticationServices(_context,_mockConfig.Object);

            var result = await _authenticationServices.GetUsers();

            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.IsType<List<User>>(result);

        }
        [Fact]
        public async void GetUsers_ShouldReturnEmptyList()
        {
            var _context = await GetMemoryDatabaseContext(0);
            _authenticationServices = new AuthenticationServices(_context, _mockConfig.Object);

            var result = await _authenticationServices.GetUsers();

            Assert.IsType<List<User>>(result);
            Assert.Empty(result);
        }
        [Fact]
        public async void Register_ShouldReturnUser()
        {
            var registerUser = new RegisterRequestDto { Email = "email@gmail.com", Username = "User", Password = "Password@123" };
            var _context = await GetMemoryDatabaseContext(1);
            _authenticationServices = new AuthenticationServices(_context, _mockConfig.Object);

            var result = await _authenticationServices.Register(registerUser);

            Assert.NotNull(result);
            Assert.IsType<User>(result);

            
        }
        [Fact]
        public async void Register_ShouldReturnArgumentException_UsernameAlreadyExists()
        {
            var registerUser = new RegisterRequestDto { Email = "email@gmail.com", Username = "User1", Password = "Password1@123" };
            var _context = await GetMemoryDatabaseContext(1);
            _authenticationServices = new AuthenticationServices(_context, _mockConfig.Object);

            await Assert.ThrowsAsync<ArgumentsException>(() => _authenticationServices.Register(registerUser));


        }
        [Fact]
        public async void Register_ShouldReturnArgumentException_InvalidEmail()
        {
            var registerUser = new RegisterRequestDto { Email = "email", Username = "User", Password = "Password1@123" };
            var _context = await GetMemoryDatabaseContext(1);
            _authenticationServices = new AuthenticationServices(_context, _mockConfig.Object);

            await Assert.ThrowsAsync<ArgumentsException>(() => _authenticationServices.Register(registerUser));



        }
        [Fact]
        public async void Register_ShouldReturnArgumentException_InvalidUsername()
        {
            var registerUser = new RegisterRequestDto { Email = "email@gmail.com", Username = "U", Password = "Password1@123" };
            var _context = await GetMemoryDatabaseContext(1);
            _authenticationServices = new AuthenticationServices(_context, _mockConfig.Object);

            await Assert.ThrowsAsync<ArgumentsException>(() => _authenticationServices.Register(registerUser));



        }
        [Fact]
        public async void Register_ShouldReturnArgumentException_InvalidPassword()
        {
            var registerUser = new RegisterRequestDto { Email = "email@gmail.com", Username = "User", Password = "Password3" };
            var _context = await GetMemoryDatabaseContext(1);
            _authenticationServices = new AuthenticationServices(_context, _mockConfig.Object);

            await Assert.ThrowsAsync<ArgumentsException>(() => _authenticationServices.Register(registerUser));

        }
        [Fact]
        public async void Login_ShallReturnUser_ValidCredentials()
        {
            var loginUser = new LoginRequestDto {Username = "User1", Password = "Password1@123" };
            var _context = await GetMemoryDatabaseContext(3);
            _authenticationServices = new AuthenticationServices(_context, _mockConfig.Object);
            _context = await HashPasswordsInTheContext(_context);

            var result = await _authenticationServices.Login(loginUser);

            Assert.NotNull(result);
            Assert.IsType<User>(result);
        }
        [Fact]
        public async void Login_ShallReturnArgumentException_InvalidCredentials()
        {
            var loginUser = new LoginRequestDto { Username = "User1", Password = "Password" };
            var _context = await GetMemoryDatabaseContext(3);
            _authenticationServices = new AuthenticationServices(_context, _mockConfig.Object);
            _context = await HashPasswordsInTheContext(_context);

            await Assert.ThrowsAsync<ArgumentsException>(() => _authenticationServices.Login(loginUser));
        }
        [Fact]
        public async void Reset_ShallReturnMessage_ValidCredentials()
        {
            var expectedMessage= "Password Reset Successfully";
            var resetUser = new ResetRequestDto { Username = "User1", OldPassword = "Password1@123" , NewPassword="Password@123"  };
            var _context = await GetMemoryDatabaseContext(3);
            _authenticationServices = new AuthenticationServices(_context, _mockConfig.Object);
            _context = await HashPasswordsInTheContext(_context);

            var result = await _authenticationServices.Reset(resetUser);

            Assert.NotNull(result);
            Assert.Equal(expectedMessage, result);


        }
        [Fact]
        public async void Reset_ShallReturnMessage_InvalidCredentials()
        {
            var resetUser = new ResetRequestDto { Username = "User1", OldPassword = "Password", NewPassword = "Password@123" };
            var _context = await GetMemoryDatabaseContext(3);
            _authenticationServices = new AuthenticationServices(_context, _mockConfig.Object);
            _context = await HashPasswordsInTheContext(_context);

            await Assert.ThrowsAsync<ArgumentsException>(() => _authenticationServices.Reset(resetUser));
        }
    }
}
