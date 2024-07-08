using Microsoft.AspNetCore.Mvc;
using JobPortal.Models;
using JobPortal.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobPortal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationController : ControllerBase
    {
        private readonly ApplicationService _applicationService;

        public ApplicationController(ApplicationService applicationService)
        {
            _applicationService = applicationService;
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

        // POST: api/application
        [HttpPost]
        public async Task<IActionResult> PostApplication(Application application)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.SelectMany(ms => ms.Value.Errors.Select(e => e.ErrorMessage));
                return BadRequest(new { message = "Validation failed", errors });
            }

            try
            {
                var newApplication = await _applicationService.CreateApplicationAsync(application);
                return Ok(new { message = "Application created successfully", application = newApplication });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Failed to create application", error = ex.Message });
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
