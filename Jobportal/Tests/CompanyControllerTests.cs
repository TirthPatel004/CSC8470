using JobPortal.Controllers;
using JobPortal.Models;
using JobPortal.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.Logging;

namespace JobPortal.Tests
{
    public class CompanyControllerTests
    {
        private readonly Mock<CompanyService> _companyServiceMock;
        private readonly Mock<ILogger<CompanyController>> _loggerMock;
        private readonly CompanyController _controller;

        public CompanyControllerTests()
        {
            _companyServiceMock = new Mock<CompanyService>();
            _loggerMock = new Mock<ILogger<CompanyController>>();
            _controller = new CompanyController(_companyServiceMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Get_ReturnsOkResult_WithCompanies()
        {
            // Arrange
            var companies = new List<Company>
            {
                new Company { Id = 1, Name = "Company1" },
                new Company { Id = 2, Name = "Company2" }
            };

            _companyServiceMock.Setup(service => service.GetAllCompanies()).Returns(companies);

            // Act
            var result = _controller.Get(); // This should not be awaited

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnCompanies = Assert.IsAssignableFrom<List<Company>>(okResult.Value);
            Assert.Equal(companies.Count, returnCompanies.Count);
        }

        // [Fact]
        // public async Task Get_ReturnsNotFoundResult_WhenCompanyDoesNotExist()
        // {
        //     // Arrange
        //     _companyServiceMock.Setup(service => service.GetCompanyById(1)).Returns((Company)null);

        //     // Act
        //     var result = await _controller.Get(1); // Await here for async method

        //     // Assert
        //     var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        //     Assert.Equal("Company not found", ((dynamic)notFoundResult.Value).message);
        // }

        [Fact]
        public async Task SignUp_ReturnsCreatedAtActionResult_WithNewCompany()
        {
            // Arrange
            var newCompany = new Company { Id = 1, Name = "NewCompany" };
            _companyServiceMock.Setup(service => service.CreateCompanyAsync(It.IsAny<Company>()))
                .ReturnsAsync(newCompany);

            var companyToCreate = new Company { Name = "NewCompany" };

            // Act
            var result = await _controller.SignUp(companyToCreate);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnedCompany = Assert.IsType<Company>(createdAtActionResult.Value);
            Assert.Equal(newCompany.Id, returnedCompany.Id);
        }

        [Fact]
        public async Task Login_ReturnsOkResult_WithAuthenticatedCompany()
        {
            // Arrange
            var company = new Company { Id = 1, Email = "test@example.com", Name = "TestCompany" };
            var loginModel = new LoginModel { Email = "test@example.com", Password = "password" };

            _companyServiceMock.Setup(service => service.Authenticate(loginModel.Email, loginModel.Password))
                .Returns(company);

            // Act
            var result = await _controller.Login(loginModel);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedCompany = Assert.IsType<Company>(okResult.Value);
            Assert.Equal(company.Email, returnedCompany.Email);
            Assert.Equal(company.Name, returnedCompany.Name);
        }

        [Fact]
        public async Task UpdateCompany_ReturnsOkResult_WithUpdatedCompany()
        {
            // Arrange
            var existingCompany = new Company { Id = 1, Name = "ExistingCompany" };
            var updatedCompany = new Company { Id = 1, Name = "UpdatedCompany" };

            _companyServiceMock.Setup(service => service.GetCompanyByIdAsync(1)).ReturnsAsync(existingCompany);
            _companyServiceMock.Setup(service => service.UpdateCompanyAsync(1, updatedCompany))
                .ReturnsAsync(updatedCompany);

            // Act
            var result = await _controller.UpdateCompany(1, updatedCompany);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedCompany = Assert.IsType<Company>(okResult.Value);
            Assert.Equal(updatedCompany.Name, returnedCompany.Name);
        }

        [Fact]
        public async Task Delete_ReturnsOkResult_WithDeletedCompany()
        {
            // Arrange
            var deletedCompany = new Company { Id = 1, Name = "DeletedCompany" };

            _companyServiceMock.Setup(service => service.DeleteCompanyAsync(1))
                .ReturnsAsync(deletedCompany);

            // Act
            var result = await _controller.Delete(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedCompany = Assert.IsType<Company>(okResult.Value);
            Assert.Equal(deletedCompany.Id, returnedCompany.Id);
        }
    }
}
