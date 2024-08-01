using JobPortal.Controllers;
using JobPortal.Models;
using JobPortal.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace JobPortal.Tests
{
    public class UserControllerTests
    {
        private readonly Mock<UserService> _userServiceMock;
        private readonly Mock<ApplicationService> _applicationServiceMock;
        private readonly UserController _controller;

        public UserControllerTests()
        {
            _userServiceMock = new Mock<UserService>();
            _applicationServiceMock = new Mock<ApplicationService>();
            _controller = new UserController(_userServiceMock.Object, _applicationServiceMock.Object);

            // Setup HttpContext and User claims for controller
            var context = new DefaultHttpContext();
            context.User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Name, "user@example.com")
            }, CookieAuthenticationDefaults.AuthenticationScheme));
            _controller.ControllerContext.HttpContext = context;
        }

        [Fact]
        public void RegisterView_ReturnsViewResult()
        {
            // Act
            var result = _controller.RegisterView();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("/Views/User/Register.cshtml", viewResult.ViewName);
        }

        [Fact]
        public async Task Register_ReturnsOkResult_WithNewUser()
        {
            // Arrange
            var newUser = new User { Id = 1, Email = "test@example.com" };
            _userServiceMock.Setup(service => service.RegisterAsync(It.IsAny<User>())).ReturnsAsync(newUser);

            var userToRegister = new User { Email = "test@example.com", Password = "password" };

            // Act
            var result = await _controller.Register(userToRegister);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedUser = Assert.IsType<User>(((dynamic)okResult.Value).user);
            Assert.Equal(newUser.Email, returnedUser.Email);
        }

        [Fact]
        public void MainLogin_ReturnsViewResult()
        {
            // Act
            var result = _controller.MainLogin();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal("/Views/User/MainLogin.cshtml", viewResult.ViewName);
        }

        [Fact]
        public async Task Login_ReturnsOkResult_WithAuthenticatedUser()
        {
            // Arrange
            var user = new User { Id = 1, Email = "test@example.com" };
            var loginModel = new LoginModel { Email = "test@example.com", Password = "password" };

            _userServiceMock.Setup(service => service.LoginAsync(loginModel.Email, loginModel.Password))
                .ReturnsAsync(user);

            // Act
            var result = await _controller.Login(loginModel);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedUser = Assert.IsType<User>(((dynamic)okResult.Value).user);
            Assert.Equal(user.Email, returnedUser.Email);
        }

        [Fact]
        public async Task UserDashboard_ReturnsViewResult_WithDashboardModel()
        {
            // Arrange
            var userProfile = new User { Id = 1, Email = "test@example.com" };
            var applications = new List<Application> { new Application { Id = 1, JobId = 1 } };

            _userServiceMock.Setup(service => service.GetUserByIdAsync(1)).ReturnsAsync(userProfile);
            _applicationServiceMock.Setup(service => service.GetApplicationsByUserIdAsync(1)).ReturnsAsync(applications);

            // Act
            var result = await _controller.UserDashboard();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<UserDashboardViewModel>(viewResult.Model);
            Assert.Equal(userProfile, model.UserProfile);
            Assert.Equal(applications, model.AppliedApplications);
        }

        [Fact]
        public async Task Update_ReturnsOkResult_WithUpdatedUser()
        {
            // Arrange
            var existingUser = new User { Id = 1, Email = "old@example.com" };
            var updatedUser = new User { Id = 1, Email = "new@example.com" };

            _userServiceMock.Setup(service => service.UpdateAsync(1, It.IsAny<User>())).ReturnsAsync(updatedUser);

            // Act
            var result = await _controller.Update(1, updatedUser);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedUser = Assert.IsType<User>(((dynamic)okResult.Value).user);
            Assert.Equal(updatedUser.Email, returnedUser.Email);
        }

        [Fact]
        public async Task Delete_ReturnsOkResult_WithDeletedUser()
        {
            // Arrange
            var deletedUser = new User { Id = 1, Email = "deleted@example.com" };

            _userServiceMock.Setup(service => service.DeleteAsync(1)).ReturnsAsync(deletedUser);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedUser = Assert.IsType<User>(((dynamic)okResult.Value).user);
            Assert.Equal(deletedUser.Email, returnedUser.Email);
        }

        [Fact]
        public void Logout_ReturnsOkResult()
        {
            // Act
            var result = _controller.Logout();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Logout successful", ((dynamic)okResult.Value).message);
        }
    }
}
