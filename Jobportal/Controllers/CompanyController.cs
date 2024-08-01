
using Microsoft.AspNetCore.Mvc;
using JobPortal.Models;
using JobPortal.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.Extensions.Logging;

namespace JobPortal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly CompanyService _companyService;
        private readonly ILogger<CompanyController> _logger;

        public CompanyController(CompanyService companyService, ILogger<CompanyController> logger)
        {
            _companyService = companyService;
            _logger = logger;
        }

        // GET: api/company
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                var companies = _companyService.GetAllCompanies();
                return Ok(companies);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve companies");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Failed to retrieve companies", error = ex.Message });
            }
        }

        // GET: api/company/5
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                var company = _companyService.GetCompanyById(id);
                if (company == null)
                    return NotFound(new { message = "Company not found" });

                return Ok(company);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve company");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Failed to retrieve company", error = ex.Message });
            }
        }

        // POST: api/company/signup
        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] Company company)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
                return BadRequest(new { message = "Validation failed", errors });
            }

            try
            {
                var newCompany = await _companyService.CreateCompanyAsync(company);
                return CreatedAtAction(nameof(Get), new { id = newCompany.Id }, newCompany);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create company");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Failed to create company", error = ex.Message });
            }
        }

         //POST: api/company/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            try
            {
                var company = _companyService.Authenticate(model.Email, model.Password);
                if (company == null)
                    return Unauthorized(new { message = "Invalid email or password" });

                // Set authentication cookie
                var claims = new[]
                {
                    new Claim(ClaimTypes.Email, company.Email),
                    new Claim(ClaimTypes.Name, company.Name),
                    new Claim("CompanyId", company.Id.ToString())

                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                return Ok(new { message = "Login successful", company });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login failed");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Login failed", error = ex.Message });
            }
        }
        
        
        // PUT: api/company/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCompany(int id, [FromBody] Company company)
        {
            if (id != company.Id)
            {
                return BadRequest(new { message = "Invalid company ID" });
            }

            try
            {
                var existingCompany = await _companyService.GetCompanyByIdAsync(id);
                if (existingCompany == null)
                {
                    return NotFound(new { message = "Company not found" });
                }

                var updatedCompany = await _companyService.UpdateCompanyAsync(id, company);
                return Ok(updatedCompany);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update company");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Failed to update company", error = ex.Message });
            }
        }

        // DELETE: api/company/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deletedCompany = await _companyService.DeleteCompanyAsync(id);
                if (deletedCompany == null)
                {
                    return NotFound(new { message = "Company not found" });
                }

                return Ok(new { message = "Company deleted successfully", company = deletedCompany });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete company");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Failed to delete company", error = ex.Message });
            }
        }

        // GET: api/company/dashboard
        [HttpGet("dashboard")]
        public async Task<IActionResult> CompanyDashboard()
        {
            try
            {
                var companyId = GetCurrentCompanyId(); 
                var company = await _companyService.GetCompanyByIdAsync(companyId);
                var jobs = await _companyService.GetJobsByCompanyIdAsync(companyId);
                var applications = await _companyService.GetApplicationsByCompanyIdAsync(companyId);
                var users = await _companyService.GetAllUsersAsync(); 

                var model = new CompanyDashboardModel
                {
                    CompanyId=companyId,
                    Company = company,
                    Jobs = jobs,
                    Applications = applications,
                    Users = users

                };

                return Ok(model); // Return the model as JSON
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load dashboard");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Failed to load dashboard", error = ex.Message });
            }
        }

        private int GetCurrentCompanyId()
        {
            var companyIdClaim = User.Claims.FirstOrDefault(c => c.Type == "CompanyId")?.Value;
            _logger.LogInformation("Company ID claim found: {CompanyIdClaim}", companyIdClaim);
            if (int.TryParse(companyIdClaim, out int companyId))
            {
                return companyId;
            }

            _logger.LogWarning("Company ID is not available in the session.");
            throw new UnauthorizedAccessException("Company ID is not available in the session.");
        }


    }
}
