using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using JobPortal.Models;
using JobPortal.Services;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace JobPortal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationController : ControllerBase
    {
        private readonly ApplicationService _applicationService;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ApplicationController(ApplicationService applicationService, IWebHostEnvironment webHostEnvironment)
        {
            _applicationService = applicationService;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: api/application
        [HttpGet]
        public async Task<IActionResult> GetApplications()
        {
            try
            {
                var applications = await _applicationService.GetAllApplicationsAsync();
                return Ok(applications);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Failed to retrieve applications", error = ex.Message });
            }
        }

        // GET: api/application/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetApplication(int id)
        {
            try
            {
                var application = await _applicationService.GetApplicationByIdAsync(id);
                if (application == null)
                    return NotFound(new { message = $"Application with ID {id} not found" });

                return Ok(application);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Failed to retrieve application with ID {id}", error = ex.Message });
            }
        }

        //[HttpPost("apply")]
        //public async Task<IActionResult> PostApplication([FromForm] Application application, IFormFile resume)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        var errors = ModelState.SelectMany(ms => ms.Value.Errors.Select(e => e.ErrorMessage));
        //        return BadRequest(new { message = "Validation failed", errors });
        //    }

        //    if (resume == null || resume.Length == 0)
        //    {
        //        return BadRequest(new { message = "Resume is required." });
        //    }

        //    try
        //    {
        //        // Save the resume file to the "resumes" directory
        //        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "resumes");
        //        Directory.CreateDirectory(uploadsFolder); // Ensure the folder exists

        //        string uniqueFileName = Guid.NewGuid().ToString() + "_" + resume.FileName;
        //        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

        //        using (var fileStream = new FileStream(filePath, FileMode.Create))
        //        {
        //            await resume.CopyToAsync(fileStream);
        //        }

        //        // Save the file path in the application record
        //        application.Resume = "/resumes/" + uniqueFileName;

        //        var newApplication = await _applicationService.CreateApplicationAsync(application);
        //        return Ok(new { message = "Application created successfully", application = newApplication });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new { message = "Failed to create application", error = ex.Message });
        //    }
        //}

        //[HttpPost("apply")]
        //public async Task<IActionResult> PostApplication([FromForm] Application application, IFormFile resume)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        var errors = ModelState.SelectMany(ms => ms.Value.Errors.Select(e => e.ErrorMessage));
        //        return BadRequest(new { message = "Validation failed", errors });
        //    }

        //    if (resume == null || resume.Length == 0)
        //    {
        //        return BadRequest(new { message = "Resume is required." });
        //    }

        //    try
        //    {
        //        // Save the resume file to the "resumes" directory
        //        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "resumes");
        //        Directory.CreateDirectory(uploadsFolder); // Ensure the folder exists

        //        // Generate a unique filename to prevent conflicts
        //        string uniqueFileName = Guid.NewGuid().ToString() + "_" + resume.FileName;
        //        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

        //        // Save the file to the specified path
        //        using (var fileStream = new FileStream(filePath, FileMode.Create))
        //        {
        //            await resume.CopyToAsync(fileStream);
        //        }

        //        // Save the path of the resume in the application record
        //        application.Resume = "/resumes/" + uniqueFileName;

        //        // Assuming you have a service to handle application creation
        //        var newApplication = await _applicationService.CreateApplicationAsync(application);
        //        return Ok(new { message = "Application created successfully", application = newApplication });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new { message = "Failed to create application", error = ex.Message });
        //    }
        //}
        [HttpPost("apply")]
        public async Task<IActionResult> PostApplication([FromForm] Application application, [FromForm] IFormFile resume)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.SelectMany(ms => ms.Value.Errors.Select(e => e.ErrorMessage));
                return BadRequest(new { message = "Validation failed", errors });
            }

            if (resume == null || resume.Length == 0)
            {
                return BadRequest(new { message = "Resume is required." });
            }

            try
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "resumes");
                Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + resume.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await resume.CopyToAsync(fileStream);
                }

                application.Resume = "/resumes/" + uniqueFileName;

                var newApplication = await _applicationService.CreateApplicationAsync(application);
                return Ok(new { message = "Application created successfully", application = newApplication });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Failed to create application", error = ex.Message });
            }
        }



        //[HttpPost("apply")]
        //public async Task<IActionResult> ApplyToJob([FromForm] Application application, IFormFile resume)
        //{
        //    if (resume != null && resume.Length > 0)
        //    {
        //        // Process the resume file (save it and set the file path)
        //        string uniqueFileName = Guid.NewGuid().ToString() + "_" + resume.FileName;
        //        string filePath = Path.Combine(_webHostEnvironment.WebRootPath, "resumes", uniqueFileName);

        //        using (var fileStream = new FileStream(filePath, FileMode.Create))
        //        {
        //            await resume.CopyToAsync(fileStream);
        //        }

        //        // Set the resume path in the application record
        //        application.Resume = "/resumes/" + uniqueFileName;
        //    }

        //    try
        //    {
        //        var newApplication = await _applicationService.CreateApplicationAsync(application);
        //        return Ok(new { message = "Application submitted successfully", application = newApplication });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new { message = "Failed to apply", error = ex.Message });
        //    }
        //}



        // GET: api/application/user/5
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetApplicationsByUserId(int userId)
        {
            try
            {
                var applications = await _applicationService.GetApplicationsByUserIdAsync(userId);
                if (applications == null || !applications.Any())
                    return NotFound(new { message = $"No applications found for user ID {userId}" });

                return Ok(applications);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Failed to retrieve applications", error = ex.Message });
            }
        }

        // PUT: api/application/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutApplication(int id, Application application)
        {
            if (id != application.Id)
                return BadRequest(new { message = "Invalid application ID" });

            try
            {
                var updatedApplication = await _applicationService.UpdateApplicationAsync(id, application);
                if (updatedApplication == null)
                    return NotFound(new { message = $"Application with ID {id} not found" });

                return Ok(new { message = "Application updated successfully", application = updatedApplication });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Failed to update application", error = ex.Message });
            }
        }

        // DELETE: api/application/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteApplication(int id)
        {
            var deletedApplication = await _applicationService.DeleteApplicationAsync(id);
            if (deletedApplication == null)
                return NotFound(new { message = $"Application with ID {id} not found" });

            return Ok(new { message = "Application deleted successfully", application = deletedApplication });
        }
    }
}
