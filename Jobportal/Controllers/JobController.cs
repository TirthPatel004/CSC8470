
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using JobPortal.Models;
using JobPortal.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace JobPortal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobController : ControllerBase
    {
        private readonly JobService _jobService;

        public JobController(JobService jobService)
        {
            _jobService = jobService ?? throw new ArgumentNullException(nameof(jobService));
        }

        // GET: api/job
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var jobs = _jobService.GetAllJobs();
                return Ok(jobs);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Failed to retrieve jobs", error = ex.Message });
            }
        }

        // GET: api/job/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var job = await _jobService.GetJobByIdAsync(id);
                if (job == null)
                    return NotFound(new { message = "Job not found" });

                return Ok(job);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Failed to retrieve job", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> PostJob([FromBody] Job jobModel)
        {
            if (jobModel.CompanyId <= 0)
            {
                return BadRequest("Invalid Company ID");
            }

            var job = new Job
            {
                Title = jobModel.Title,
                Location = jobModel.Location,
                Description = jobModel.Description,
                CompanyId = jobModel.CompanyId,
                PostedDate = DateTime.UtcNow
            };

            var result = await _jobService.CreateJobAsync(job);
            if (result != null)
            {
                return Ok(new { message = "Job posted successfully" });
            }
            else
            {
                return StatusCode(500, "Failed to post job");
            }
        }



        // PUT: api/job/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Job job)
        {
            if (id != job.Id)
            {
                return BadRequest(new { message = "Invalid job ID" });
            }

            var existingJob = await _jobService.GetJobByIdAsync(id);
            if (existingJob == null)
            {
                return NotFound();
            }

            try
            {
                var updatedJob = await _jobService.UpdateJobAsync(id, job);
                return Ok(updatedJob);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Failed to update job", error = ex.Message });
            }
        }

        // DELETE: api/job/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deletedJob = await _jobService.DeleteJobAsync(id);
            if (deletedJob == null)
                return NotFound(new { message = "Job not found" });

            return Ok(new { message = "Job deleted successfully", job = deletedJob });
        }
    }
}
