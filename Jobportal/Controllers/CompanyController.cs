using Microsoft.AspNetCore.Mvc;
using JobPortal.Models; 
using JobPortal.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace JobPortal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly CompanyService _companyService;

        public CompanyController(CompanyService companyService)
        {
            _companyService = companyService;
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
                return BadRequest(new { message = "Failed to retrieve companies", error = ex.Message });
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
                return BadRequest(new { message = "Failed to retrieve company", error = ex.Message });
            }
        }

        // POST: api/company/signup
        [HttpPost("signup")]
        public async Task<IActionResult> SignUp(Company company)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.SelectMany(ms => ms.Value.Errors.Select(e => e.ErrorMessage));
                return BadRequest(new { message = "Validation failed", errors });
            }

            try
            {
                var newCompany = await _companyService.CreateCompanyAsync(company);
                return Ok(new { message = "Company created successfully!", company = newCompany });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Failed to create company", error = ex.Message });
            }
        }

        // POST: api/company/login
        [HttpPost("login")]
        public IActionResult Login(LoginModel model) // Use LoginModel directly
        {
            var company = _companyService.Authenticate(model.Email, model.Password);

            if (company == null)
                return Unauthorized(new { message = "Invalid email or password" });

            return Ok(new { message = "Login successful", company });
        }

        // PUT: api/company/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCompany(int id, Company company)
        {
            if (id != company.Id)
            {
                return BadRequest(new { message = "Invalid company ID" });
            }

            var existingCompany = await _companyService.GetCompanyByIdAsync(id);
            if (existingCompany == null)
            {
                return NotFound();
            }

            try
            {
                var updatedCompany = await _companyService.UpdateCompanyAsync(id, company);
                return Ok(updatedCompany);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Failed to update company", error = ex.Message });
            }
        }

        // DELETE: api/company/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deletedCompany = await _companyService.DeleteCompanyAsync(id);
            if (deletedCompany == null)
                return NotFound(new { message = "Company not found" });

            return Ok(new { message = "Company deleted successfully", company = deletedCompany });
        }
    }
}
