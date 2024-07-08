using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JobPortal.Controllers;
using JobPortal.Models;
using JobPortal.Services;
using JobPortal.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace JobPortal.Tests
{
    public class ApplicationControllerTests
    {
        private readonly ApplicationService _applicationService;
        private readonly ApplicationController _applicationController;
        private readonly DbContextOptions<ApplicationDbContext> _options;

        public ApplicationControllerTests()
        {
            // Setup in-memory database
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "JobPortalTestDb")
                .Options;

            // Seed data
            using (var context = new ApplicationDbContext(_options))
            {
                context.Applications.AddRange(
                    new Application { Id = 1, JobId = 1, UserId = 1, AppliedDate = new DateTime(2024, 7, 4, 8, 0, 0), Resume = "Updated Base64 encoded resume data" },
                    new Application { Id = 2, JobId = 1, UserId = 1, AppliedDate = new DateTime(2024, 7, 4, 8, 0, 0), Resume = "Updated Base64 encoded resume data" }
                );
                context.SaveChanges();
            }

            _applicationService = new ApplicationService(new ApplicationDbContext(_options));
            _applicationController = new ApplicationController(_applicationService);
        }

        [Fact]
        public async Task GetApplications_ReturnsAllApplications()
        {
            // Act
            var result = await _applicationController.GetApplications() as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result);
            var applications = result.Value as IEnumerable<Application>;
            Assert.Equal(2, applications.Count());
        }

        [Fact]
        public async Task GetApplication_ReturnsApplicationById()
        {
            // Arrange
            var applicationId = 1;

            // Act
            var result = await _applicationController.GetApplication(applicationId) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result);
            var application = result.Value as Application;
            Assert.NotNull(application);
            Assert.Equal(applicationId, application.Id);
        }

        [Fact]
        public async Task PostApplication_CreatesNewApplication()
        {
            // Arrange
            var newApplication = new Application
            {
                JobId = 2,
                UserId = 2,
                AppliedDate = new DateTime(2024, 7, 5, 8, 0, 0),
                Resume = "New Base64 encoded resume data"
            };

            // Act
            var result = await _applicationController.PostApplication(newApplication) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result);
            var createdApplication = result.Value.GetType().GetProperty("application").GetValue(result.Value, null) as Application;
            Assert.NotNull(createdApplication);
            Assert.Equal(newApplication.Resume, createdApplication.Resume);
        }

        [Fact]
        public async Task PutApplication_UpdatesExistingApplication()
        {
            // Arrange
            var applicationId = 1;
            var updatedApplication = new Application
            {
                Id = applicationId,
                JobId = 1,
                UserId = 1,
                AppliedDate = new DateTime(2024, 7, 4, 8, 0, 0),
                Resume = "Updated resume data"
            };

            // Act
            var result = await _applicationController.PutApplication(applicationId, updatedApplication) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result);
            var application = result.Value.GetType().GetProperty("application").GetValue(result.Value, null) as Application;
            Assert.NotNull(application);
            Assert.Equal(updatedApplication.Resume, application.Resume);
        }

        [Fact]
        public async Task DeleteApplication_RemovesApplication()
        {
            // Arrange
            var applicationId = 2;

            // Act
            var result = await _applicationController.DeleteApplication(applicationId) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result);
            var deletedApplication = result.Value.GetType().GetProperty("application").GetValue(result.Value, null) as Application;
            Assert.NotNull(deletedApplication);
            Assert.Equal(applicationId, deletedApplication.Id);
        }
    }
}
