
using JobPortal.Models;
using JobPortal.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http; // For accessing session

namespace JobPortal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly UserService _userService;
        private readonly ApplicationService _applicationService;

        public UserController(UserService userService, ApplicationService applicationService)
        {
            _userService = userService;
            _applicationService = applicationService;
        }

        // GET: /api/user/register
        [HttpGet("register")]
        public IActionResult RegisterView()
        {
            return View("/Views/User/Register.cshtml"); // Return the Register.cshtml view
        }

        // POST: /api/user/register
        [HttpPost("register")]
        public async Task<IActionResult> Register(User user)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.SelectMany(ms => ms.Value.Errors.Select(e => e.ErrorMessage));
                return BadRequest(new { message = "Validation failed", errors });
            }

            try
            {
                var newUser = await _userService.RegisterAsync(user);
                return Ok(new { message = "User registered successfully!", user = newUser });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Registration failed", error = ex.Message });
            }
        }

        // GET: /api/user/login
        [HttpGet("login")]
        public IActionResult MainLogin()
        {
            return View("/Views/User/MainLogin.cshtml");
        }

        // POST: /api/user/login
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Return validation errors if model state is invalid
            }

            try
            {
                var user = await _userService.LoginAsync(model.Email, model.Password);
                if (user == null)
                    return Unauthorized(new { message = "Invalid credentials" });

                // Create authentication ticket
                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
                };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                // Sign in the user
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                return Ok(new { message = "Login successful", user });
            }
            catch (Exception ex)
            {
                // Log the exception (you might use a logging framework like Serilog or NLog)
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(500, new { message = "An error occurred while processing your request" });
            }
        }

        // GET: /api/user/dashboard
        [HttpGet("dashboard")]
        public async Task<IActionResult> UserDashboard()
        {
            try
            {
                if (!User.Identity.IsAuthenticated)
                {
                    return Unauthorized(new { message = "User is not logged in" });
                }

                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

                var userProfile = await _userService.GetUserByIdAsync(userId);
                var appliedApplications = await _applicationService.GetApplicationsByUserIdAsync(userId);

                var model = new UserDashboardViewModel
                {
                    UserProfile = userProfile,
                    AppliedApplications = appliedApplications ?? new List<Application>()
                };

                return View("/Views/User/UserDashboard.cshtml", model);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Failed to load user dashboard", error = ex.Message });
            }
        }

        // PUT: /api/user/update/5
        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] User user)
        {
            if (id != user.Id)
                return BadRequest(new { message = "Invalid user ID" });

            try
            {
                var updatedUser = await _userService.UpdateAsync(id, user);
                if (updatedUser == null)
                    return NotFound(new { message = "User not found" });

                return Ok(new { message = "User updated successfully", user = updatedUser });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Update failed", error = ex.Message });
            }
        }

        // DELETE: /api/user/delete/5
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deletedUser = await _userService.DeleteAsync(id);
            if (deletedUser == null)
                return NotFound(new { message = "User not found" });

            return Ok(new { message = "User deleted successfully", user = deletedUser });
        }
        // POST: /api/user/logout
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            try
            {
                // Sign out the user
                HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                // Clear the session
                HttpContext.Session.Clear();

                return Ok(new { message = "Logout successful" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Logout failed", error = ex.Message });
            }
        }

    }
}
