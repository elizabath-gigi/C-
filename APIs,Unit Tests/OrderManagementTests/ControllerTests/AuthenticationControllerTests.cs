using Microsoft.AspNetCore.Mvc;
using Moq;
using OrderManagement.Controllers;
using OrderManagement.DTOs;
using OrderManagement.Interface;
using OrderManagement.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Reflection.Metadata.BlobBuilder;

namespace OrderManagementTests.ControllerTests
{
    public class AuthenticationControllerTests
    {
        private readonly Mock<IAuthenticationServices> _mockIAuthenticationService;
        private readonly UserController _userController;
        public AuthenticationControllerTests()

        {
            _mockIAuthenticationService = new Mock<IAuthenticationServices>();
            _userController = new UserController(_mockIAuthenticationService.Object);
        }
        [Fact]
        public async void GetUsers_ShouldReturnOkUsers()
        {
            var users = new List<User>
            {
                new User { Id = 1, Email = "email1@gmail.com" , Username ="User1" , Password = "User@1"},
                new User { Id = 2, Email = "email2@gmail.com" , Username ="User2" , Password = "User@2"}
            };
            _mockIAuthenticationService.Setup(service => service.GetUsers()).ReturnsAsync(users);


            var result = await _userController.GetUsers();


            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<List<User>>(okResult.Value);
            Assert.Equal(2, returnValue.Count);
        }
        [Fact]
        public async void GetUsers_ShouldReturnNotFoundWithMessage()
        {
            var users = new List<User>();
            
            _mockIAuthenticationService.Setup(service => service.GetUsers()).ReturnsAsync(users);


            var result = await _userController.GetUsers();


            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("No users found", notFoundResult.Value);
            
        }
        [Fact]
        public async void Register_ShouldReturnOkWithUser()
        {
            var user = new RegisterRequestDto {  Email = "email1@gmail.com", Username = "User1", Password = "User@1" };
            var userWithId = new User { Id = 1, Email = "email1@gmail.com", Username = "User1", Password = "User@1" };
            _mockIAuthenticationService.Setup(service => service.Register(user)).ReturnsAsync(userWithId);

            var result = await _userController.Register(user);


            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<User>(okResult.Value);
            Assert.Equal(userWithId.Id,returnValue.Id);
            Assert.Equal(userWithId.Email, returnValue.Email);
            Assert.Equal(userWithId.Username, returnValue.Username);
            Assert.Equal(userWithId.Password, returnValue.Password);
        }
        [Fact]
        public async Task Login_ShouldReturnOkWithToken()
        {
            var user = new LoginRequestDto { Username = "User1", Password = "User@1" };
            var userWithId = new User { Id = 1, Email = "email1@gmail.com", Username = "User1", Password = "User@1" };
            var sampleToken = "sampleToken";

            _mockIAuthenticationService.Setup(service => service.Login(user))
                                      .ReturnsAsync(userWithId);
            _mockIAuthenticationService.Setup(service => service.GenerateJSONWebToken(userWithId))
                                      .Returns(sampleToken);

           
            var result = await _userController.Login(user);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<string>(okResult.Value);
            Assert.Equal(sampleToken, returnValue);
        }
        [Fact]
        public async void ResetPassword_ShouldReturnOkWithMessage()
        {
            var user = new ResetRequestDto { Username = "User1", OldPassword = "User@1",NewPassword="User@2" };
            var message = "Reset Successfully";

            _mockIAuthenticationService.Setup(service => service.Reset(user))
                                      .ReturnsAsync(message);

            var result = await _userController.ResetPassword(user);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<string>(okResult.Value);
            Assert.Equal(message, returnValue);

        }
        
    }
}
