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
using Microsoft.EntityFrameworkCore; // Add this using directive

public class JobControllerTests
{
    private readonly JobService _jobService;
    private readonly JobController _jobController;

    public JobControllerTests()
    {
        // Setup in-memory database
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "JobPortalTestDb")
            .Options;

        _jobService = new JobService(new ApplicationDbContext(options));
        _jobController = new JobController(_jobService);
    }

    [Fact]
    public void Get_ReturnsAllJobs()
    {
        // Act
        var result = _jobController.Get() as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);
        var jobs = result.Value as IEnumerable<Job>;
        Assert.NotNull(jobs);
        Assert.Equal(2, jobs.Count()); 
    }

    [Fact]
    public async Task Get_ReturnsJobById()
    {
        // Arrange
        var jobId = 1;

        // Act
        var result = await _jobController.Get(jobId) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);
        var job = result.Value as Job;
        Assert.NotNull(job);
        Assert.Equal(jobId, job.Id);
    }

    [Fact]
    public async Task Post_CreatesNewJob()
    {
        // Arrange
        var newJob = new Job
        {
            Title = "New Job Title",
            Description = "New Job Description",
            Location = "New Job Location",
            PostedDate = DateTime.Now,
            CompanyId = 1
        };

        // Act
        var result = await _jobController.Post(newJob) as CreatedAtActionResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<CreatedAtActionResult>(result);
        var createdJob = result.Value.GetType().GetProperty("job").GetValue(result.Value, null) as Job;
        Assert.NotNull(createdJob);
        Assert.Equal(newJob.Title, createdJob.Title);
    }

    [Fact]
    public async Task Put_UpdatesExistingJob()
    {
        // Arrange
        var jobId = 1;
        var updatedJob = new Job
        {
            Id = jobId,
            Title = "Updated Job Title Again",
            Description = "Updated Job Description Again",
            Location = "Updated Job Location Again",
            PostedDate = DateTime.Now,
            CompanyId = 1
        };

        // Act
        var result = await _jobController.Put(jobId, updatedJob) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);
        var job = result.Value as Job;
        Assert.NotNull(job);
        Assert.Equal(updatedJob.Title, job.Title);
    }

    [Fact]
    public async Task Delete_RemovesJob()
    {
        // Arrange
        var jobId = 2;

        // Act
        var result = await _jobController.Delete(jobId) as OkObjectResult;

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);
        var deletedJob = result.Value.GetType().GetProperty("job").GetValue(result.Value, null) as Job;
        Assert.NotNull(deletedJob);
        Assert.Equal(jobId, deletedJob.Id);
    }
}
