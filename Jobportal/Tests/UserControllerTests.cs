using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JobPortal.Controllers;
using JobPortal.Models;
using JobPortal.Services;
using Microsoft.AspNetCore.Mvc;
using Xunit;
using JobPortal.Data;
using Microsoft.EntityFrameworkCore;

public class UserControllerTests
{
    private readonly UserService _userService;
    private readonly UserController _userController;

    public UserControllerTests()
    {
        // Setup in-memory database for testing
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        var dbContext = new ApplicationDbContext(options);
        SeedTestData(dbContext); // Optional: Seed test data if needed

        _userService = new UserService(dbContext);
        _userController = new UserController(_userService);
    }

    // Helper method to seed test data (optional)
    private void SeedTestData(ApplicationDbContext dbContext)
    {
        dbContext.Users.AddRange(new List<User>
        {
            new User { Id = 4, FirstName = "John", LastName = "Doe", Email = "johndoe@example.com", Password = "Password123" },
            new User { Id = 5, FirstName = "Jane", LastName = "Doe", Email = "janedoe@example.com", Password = "Password456" }
        });

        dbContext.SaveChanges();
    }

    [Fact]
    public async Task Register_CreatesNewUser()
    {
        // Arrange
        var newUser = new User
        {
            FirstName = "Alice",
            LastName = "Smith",
            Email = "alicesmith@example.com",
            Password = "NewPassword123"
        };

        // Act
        var result = await _userController.Register(newUser) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);
        var createdUser = result.Value.GetType().GetProperty("user").GetValue(result.Value, null) as User;
        Assert.NotNull(createdUser);
        Assert.Equal(newUser.Email, createdUser.Email);
    }

    [Fact]
    public async Task Login_ReturnsUserOnValidCredentials()
    {
        // Arrange
        var loginModel = new LoginModel
        {
            Email = "johndoe@example.com",
            Password = "Password123"
        };

        // Act
        var result = await _userController.Login(loginModel) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);
        var user = result.Value.GetType().GetProperty("user").GetValue(result.Value, null) as User;
        Assert.NotNull(user);
        Assert.Equal(loginModel.Email, user.Email);
    }

    [Fact]
    public async Task Login_ReturnsUnauthorizedOnInvalidCredentials()
    {
        // Arrange
        var loginModel = new LoginModel
        {
            Email = "wrong@example.com",
            Password = "WrongPassword"
        };

        // Act
        var result = await _userController.Login(loginModel) as UnauthorizedObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<UnauthorizedObjectResult>(result);
        var message = result.Value.GetType().GetProperty("message").GetValue(result.Value, null) as string;
        Assert.Equal("Invalid credentials", message);
    }

    [Fact]
    public async Task Update_UpdatesExistingUser()
    {
        // Arrange
        var userId = 4;
        var updatedUser = new User
        {
            Id = userId,
            FirstName = "Jane",
            LastName = "Updated",
            Email = "janedoe@example.com",
            Password = "NewPassword123"
        };

        // Act
        var result = await _userController.Update(userId, updatedUser) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);
        var user = result.Value.GetType().GetProperty("user").GetValue(result.Value, null) as User;
        Assert.NotNull(user);
        Assert.Equal(updatedUser.LastName, user.LastName);
    }

    [Fact]
    public async Task Delete_RemovesUser()
    {
        // Arrange
        var userId = 5;

        // Act
        var result = await _userController.Delete(userId) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);
        var deletedUser = result.Value.GetType().GetProperty("user").GetValue(result.Value, null) as User;
        Assert.NotNull(deletedUser);
        Assert.Equal(userId, deletedUser.Id);
    }
}
