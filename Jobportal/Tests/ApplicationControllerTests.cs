// using Microsoft.AspNetCore.Hosting;
// using Microsoft.AspNetCore.Http;
// using Microsoft.AspNetCore.Mvc;
// using Moq;
// using JobPortal.Controllers;
// using JobPortal.Models;
// using JobPortal.Services;
// using System;
// using System.Collections.Generic;
// using System.IO;
// using System.Linq;
// using System.Threading.Tasks;
// using Xunit;

// namespace JobPortal.Tests
// {
//     public class ApplicationControllerTests
//     {
//         private readonly Mock<ApplicationService> _mockApplicationService;
//         private readonly Mock<IWebHostEnvironment> _mockWebHostEnvironment;
//         private readonly ApplicationController _controller;

//         public ApplicationControllerTests()
//         {
//             _mockApplicationService = new Mock<ApplicationService>();
//             _mockWebHostEnvironment = new Mock<IWebHostEnvironment>();
//             _controller = new ApplicationController(_mockApplicationService.Object, _mockWebHostEnvironment.Object);
//         }

//         [Fact]
//         public async Task GetApplications_ReturnsOkResult_WithApplications()
//         {
//             // Arrange
//             var applications = new List<Application>
//             {
//                 new Application { Id = 1, Resume = "resume1.pdf" },
//                 new Application { Id = 2, Resume = "resume2.pdf" }
//             };

//             _mockApplicationService.Setup(service => service.GetAllApplicationsAsync())
//                 .ReturnsAsync(applications);

//             // Act
//             var result = await _controller.GetApplications();

//             // Assert
//             var okResult = Assert.IsType<OkObjectResult>(result);
//             var returnValue = Assert.IsAssignableFrom<IEnumerable<Application>>(okResult.Value);
//             Assert.Equal(2, returnValue.Count());
//         }

//         [Fact]
//         public async Task GetApplication_ReturnsNotFound_WhenApplicationDoesNotExist()
//         {
//             // Arrange
//             var id = 1;
//             _mockApplicationService.Setup(service => service.GetApplicationByIdAsync(id))
//                 .ReturnsAsync((Application)null);

//             // Act
//             var result = await _controller.GetApplication(id);

//             // Assert
//             var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
//             Assert.Equal($"Application with ID {id} not found", ((dynamic)notFoundResult.Value).message);
//         }

//         [Fact]
//         public async Task PostApplication_ReturnsBadRequest_WhenResumeIsNull()
//         {
//             // Arrange
//             var application = new Application { Id = 1 };
//             IFormFile resume = null;

//             // Act
//             var result = await _controller.PostApplication(application, resume);

//             // Assert
//             var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
//             Assert.Equal("Resume is required.", ((dynamic)badRequestResult.Value).message);
//         }

//         [Fact]
//         public async Task PutApplication_ReturnsOkResult_WithUpdatedApplication()
//         {
//             // Arrange
//             var id = 1;
//             var application = new Application { Id = id };

//             _mockApplicationService.Setup(service => service.UpdateApplicationAsync(id, application))
//                 .ReturnsAsync(application);

//             // Act
//             var result = await _controller.PutApplication(id, application);

//             // Assert
//             var okResult = Assert.IsType<OkObjectResult>(result);
//             var returnValue = Assert.IsType<Application>(((dynamic)okResult.Value).application);
//             Assert.Equal(id, returnValue.Id);
//         }

//         [Fact]
//         public async Task DeleteApplication_ReturnsNotFound_WhenApplicationDoesNotExist()
//         {
//             // Arrange
//             var id = 1;
//             _mockApplicationService.Setup(service => service.DeleteApplicationAsync(id))
//                 .ReturnsAsync((Application)null);

//             // Act
//             var result = await _controller.DeleteApplication(id);

//             // Assert
//             var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
//             Assert.Equal($"Application with ID {id} not found", ((dynamic)notFoundResult.Value).message);
//         }
//     }
// }
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using JobPortal.Controllers;
using JobPortal.Models;
using JobPortal.Services;
using Xunit;
using Microsoft.AspNetCore.Hosting;

namespace JobPortal.Tests
{
    public class ApplicationControllerTests
    {
        private readonly ApplicationController _controller;
        private readonly Mock<ApplicationService> _mockApplicationService;
        private readonly Mock<IWebHostEnvironment> _mockWebHostEnvironment;

        public ApplicationControllerTests()
        {
            _mockApplicationService = new Mock<ApplicationService>();
            _mockWebHostEnvironment = new Mock<IWebHostEnvironment>();

            // Provide mock for IWebHostEnvironment
            _controller = new ApplicationController(_mockApplicationService.Object, _mockWebHostEnvironment.Object);
        }

        [Fact]
        public async Task GetApplications_ReturnsOkResult_WithApplications()
        {
            // Arrange
            var applications = new List<Application>
            {
                new Application { Id = 1, JobId = 1, UserId = 1, Resume = "resume1.pdf" },
                new Application { Id = 2, JobId = 2, UserId = 2, Resume = "resume2.pdf" }
            };

            _mockApplicationService.Setup(service => service.GetAllApplicationsAsync())
                .ReturnsAsync(applications);

            // Act
            var result = await _controller.GetApplications();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedApplications = Assert.IsAssignableFrom<IEnumerable<Application>>(okResult.Value);
            Assert.Equal(2, returnedApplications.Count());
        }

        [Fact]
        public async Task GetApplication_ReturnsNotFound_WhenApplicationDoesNotExist()
        {
            // Arrange
            _mockApplicationService.Setup(service => service.GetApplicationByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Application)null);

            // Act
            var result = await _controller.GetApplication(1);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task PostApplication_ReturnsBadRequest_WhenResumeIsNull()
        {
            // Arrange
            var application = new Application
            {
                Id = 1,
                JobId = 1,
                UserId = 1,
                Resume = null // Invalid resume
            };

            // Act
            var result = await _controller.PostApplication(application, null); // Passing null for IFormFile

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task PutApplication_ReturnsOkResult_WithUpdatedApplication()
        {
            // Arrange
            var application = new Application
            {
                Id = 1,
                JobId = 1,
                UserId = 1,
                Resume = "updatedResume.pdf"
            };

            _mockApplicationService.Setup(service => service.UpdateApplicationAsync(It.IsAny<int>(), It.IsAny<Application>()))
                .ReturnsAsync(application);

            // Act
            var result = await _controller.PutApplication(1, application);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var updatedApplication = Assert.IsType<Application>(okResult.Value);
            Assert.Equal("updatedResume.pdf", updatedApplication.Resume);
        }

        [Fact]
        public async Task DeleteApplication_ReturnsOkResult_WhenApplicationExists()
        {
            // Arrange
            var application = new Application
            {
                Id = 1,
                JobId = 1,
                UserId = 1,
                Resume = "resume.pdf"
            };

            _mockApplicationService.Setup(service => service.DeleteApplicationAsync(It.IsAny<int>()))
                .ReturnsAsync(application);

            // Act
            var result = await _controller.DeleteApplication(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var deletedApplication = Assert.IsType<Application>(okResult.Value);
            Assert.Equal(1, deletedApplication.Id);
        }
    }
}
