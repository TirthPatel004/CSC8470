
using Microsoft.AspNetCore.Mvc;
using JobPortal.Models; 
using JobPortal.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using static JobPortal.Models.Company;

namespace JobPortal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }

        // POST: api/user/register
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

        // POST: api/user/login
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Return validation errors if model state is invalid
            }

            var user = await _userService.LoginAsync(model.Email, model.Password);
            if (user == null)
                return Unauthorized(new { message = "Invalid credentials" });

            return Ok(new { message = "Login successful", user });
        }

        // PUT: api/user/update/5
        [HttpPut("update/{id}")]
        public async Task<IActionResult> Update(int id, User user)
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

        // DELETE: api/user/delete/5
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deletedUser = await _userService.DeleteAsync(id);
            if (deletedUser == null)
                return NotFound(new { message = "User not found" });

            return Ok(new { message = "User deleted successfully", user = deletedUser });
        }
    }
}
