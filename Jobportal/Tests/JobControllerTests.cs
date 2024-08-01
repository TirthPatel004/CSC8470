using Microsoft.AspNetCore.Mvc;
using Moq;
using JobPortal.Controllers;
using JobPortal.Models;
using JobPortal.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace JobPortal.Tests
{
    public class JobControllerTests
    {
        private readonly Mock<JobService> _mockJobService;
        private readonly JobController _controller;

        public JobControllerTests()
        {
            _mockJobService = new Mock<JobService>();
            _controller = new JobController(_mockJobService.Object);
        }

        [Fact]
        public void Get_ReturnsOkResult_WithJobs()
        {
            // Arrange
            var jobs = new List<Job>
            {
                new Job { Id = 1, Title = "Software Engineer", Location = "Sydney", Description = "Develop software", CompanyId = 1 },
                new Job { Id = 2, Title = "Data Scientist", Location = "Melbourne", Description = "Analyze data", CompanyId = 2 }
            };

            _mockJobService.Setup(service => service.GetAllJobs())
                .Returns(jobs);

            // Act
            var result = _controller.Get();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<Job>>(okResult.Value);
            Assert.Equal(2, returnValue.Count());
        }

        [Fact]
        public async Task Get_ReturnsNotFound_WhenJobDoesNotExist()
        {
            // Arrange
            var id = 1;
            _mockJobService.Setup(service => service.GetJobByIdAsync(id))
                .ReturnsAsync((Job)null);

            // Act
            var result = await _controller.Get(id);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Job not found", ((dynamic)notFoundResult.Value).message);
        }

        [Fact]
        public async Task PostJob_ReturnsBadRequest_WhenCompanyIdIsInvalid()
        {
            // Arrange
            var job = new Job { Title = "Software Engineer", Location = "Sydney", Description = "Develop software", CompanyId = 0 };

            // Act
            var result = await _controller.PostJob(job);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid Company ID", badRequestResult.Value);
        }

        [Fact]
        public async Task PostJob_ReturnsOkResult_WhenJobIsPostedSuccessfully()
        {
            // Arrange
            var job = new Job { Title = "Software Engineer", Location = "Sydney", Description = "Develop software", CompanyId = 1 };
            _mockJobService.Setup(service => service.CreateJobAsync(It.IsAny<Job>()))
                .ReturnsAsync(job);

            // Act
            var result = await _controller.PostJob(job);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Job posted successfully", ((dynamic)okResult.Value).message);
        }

        [Fact]
        public async Task Put_ReturnsBadRequest_WhenIdsDoNotMatch()
        {
            // Arrange
            var id = 1;
            var job = new Job { Id = 2 }; // ID does not match

            // Act
            var result = await _controller.Put(id, job);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid job ID", ((dynamic)badRequestResult.Value).message);
        }

        [Fact]
        public async Task Put_ReturnsNotFound_WhenJobDoesNotExist()
        {
            // Arrange
            var id = 1;
            var job = new Job { Id = id };
            _mockJobService.Setup(service => service.GetJobByIdAsync(id))
                .ReturnsAsync((Job)null);

            // Act
            var result = await _controller.Put(id, job);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Put_ReturnsOkResult_WhenJobIsUpdated()
        {
            // Arrange
            var id = 1;
            var job = new Job { Id = id, Title = "Updated Job Title" };
            _mockJobService.Setup(service => service.GetJobByIdAsync(id))
                .ReturnsAsync(job);
            _mockJobService.Setup(service => service.UpdateJobAsync(id, job))
                .ReturnsAsync(job);

            // Act
            var result = await _controller.Put(id, job);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<Job>(okResult.Value);
            Assert.Equal(job.Title, returnValue.Title);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenJobDoesNotExist()
        {
            // Arrange
            var id = 1;
            _mockJobService.Setup(service => service.DeleteJobAsync(id))
                .ReturnsAsync((Job)null);

            // Act
            var result = await _controller.Delete(id);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("Job not found", ((dynamic)notFoundResult.Value).message);
        }

        [Fact]
        public async Task Delete_ReturnsOkResult_WhenJobIsDeleted()
        {
            // Arrange
            var id = 1;
            var job = new Job { Id = id, Title = "Software Engineer" };
            _mockJobService.Setup(service => service.DeleteJobAsync(id))
                .ReturnsAsync(job);

            // Act
            var result = await _controller.Delete(id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<Job>(((dynamic)okResult.Value).job);
            Assert.Equal(job.Id, returnValue.Id);
        }
    }
}
