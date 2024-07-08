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

public class CompanyControllerTests
{
    private readonly CompanyService _companyService;
    private readonly CompanyController _companyController;
    private readonly DbContextOptions<ApplicationDbContext> _options;

    public CompanyControllerTests()
    {
        // Setup in-memory database
        _options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "JobPortalTestDb")
            .Options;

        // Seed data
        using (var context = new ApplicationDbContext(_options))
        {
            context.Companies.AddRange(
                new Company { Id = 1, Name = "Updated ABC Company", Address = "456 Oak Avenue", Website = "https://www.updatedabccompany.com", Password = "updatedpassword456", Email = "" },
                new Company { Id = 2, Name = "Tech Solutions Inc.", Address = "123 Tech Street, Techland", Website = "https://techsolutions.com", Password = "securepassword", Email = "info@techsolutions.com" },
                new Company { Id = 4, Name = "ABC Company", Address = "123 Main Street", Website = "https://www.abccompany.com", Password = "password123", Email = "contact@hjkmpany.com" },
                new Company { Id = 5, Name = "ABC Company", Address = "123 Main Street", Website = "https://www.abccompany.com", Password = "password123", Email = "contact@hnmmpany.com" },
                new Company { Id = 6, Name = "ABC Company", Address = "123 Main Street", Website = "https://www.abccompany.com", Password = "password123", Email = "contact@hnmmpany.com" }
            );
            context.SaveChanges();
        }

        _companyService = new CompanyService(new ApplicationDbContext(_options));
        _companyController = new CompanyController(_companyService);
    }

    [Fact]
    public void Get_ReturnsAllCompanies()
    {
        // Act
        var result = _companyController.Get() as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);
        var companies = result.Value as IEnumerable<Company>;
        Assert.NotNull(companies);
        Assert.Equal(5, companies.Count()); // Adjust the expected count based on your actual data
    }

    [Fact]
    public void Get_ReturnsCompanyById()
    {
        // Arrange
        var companyId = 2;

        // Act
        var result = _companyController.Get(companyId) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);
        var company = result.Value as Company;
        Assert.NotNull(company);
        Assert.Equal(companyId, company.Id);
    }

    [Fact]
    public async Task SignUp_CreatesNewCompany()
    {
        // Arrange
        var newCompany = new Company
        {
            Name = "New Company",
            Address = "789 New Street",
            Website = "https://newcompany.com",
            Password = "newpassword",
            Email = "contact@newcompany.com"
        };

        // Act
        var result = await _companyController.SignUp(newCompany) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);
        var createdCompany = result.Value.GetType().GetProperty("company").GetValue(result.Value, null) as Company;
        Assert.NotNull(createdCompany);
        Assert.Equal(newCompany.Name, createdCompany.Name);
    }

    [Fact]
    public void Login_ReturnsCompanyForValidCredentials()
    {
        // Arrange
        var loginModel = new LoginModel
        {
            Email = "info@techsolutions.com",
            Password = "securepassword"
        };

        // Act
        var result = _companyController.Login(loginModel) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);
        var company = result.Value.GetType().GetProperty("company").GetValue(result.Value, null) as Company;
        Assert.NotNull(company);
        Assert.Equal(loginModel.Email, company.Email);
    }

    [Fact]
    public async Task UpdateCompany_UpdatesExistingCompany()
    {
        // Arrange
        var companyId = 2;
        var updatedCompany = new Company
        {
            Id = companyId,
            Name = "Updated Tech Solutions Inc.",
            Address = "123 Tech Street, Techland",
            Website = "https://updatedtechsolutions.com",
            Password = "updatedpassword",
            Email = "updated@techsolutions.com"
        };

        // Act
        var result = await _companyController.UpdateCompany(companyId, updatedCompany) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);
        var company = result.Value as Company;
        Assert.NotNull(company);
        Assert.Equal(updatedCompany.Name, company.Name);
    }

    [Fact]
    public async Task Delete_RemovesCompany()
    {
        // Arrange
        var companyId = 5;

        // Act
        var result = await _companyController.Delete(companyId) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);
        var deletedCompany = result.Value.GetType().GetProperty("company").GetValue(result.Value, null) as Company;
        Assert.NotNull(deletedCompany);
        Assert.Equal(companyId, deletedCompany.Id);
    }
}
