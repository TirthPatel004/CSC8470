using JobPortal.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using JobPortal.Data;
namespace JobPortal.Services
{
    public class UserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<User> RegisterAsync(User user)
        {
            try
            {
                // Check if email is already registered
                if (await _context.Users.AnyAsync(u => u.Email == user.Email))
                    throw new InvalidOperationException("Email is already registered");

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return user;
            }
            catch (Exception ex)
            {
                // Log the exception or handle as needed
                throw new Exception("Failed to register user", ex);
            }
        }

        public async Task<User> LoginAsync(string email, string password)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.Password == password);

                if (user == null)
                {
                    throw new Exception("User not found or invalid credentials");
                }

                return user;
            }
            catch (Exception ex)
            {
                // Log the exception or handle as needed
                throw new Exception("Failed to login user", ex);
            }
        }


       public async Task<User> UpdateAsync(int id, User user)
{
    try
    {
        var existingUser = await _context.Users.FindAsync(id);
        if (existingUser == null)
            return null; // User not found, return null or handle as appropriate

        // Update user properties
        existingUser.FirstName = user.FirstName;
        existingUser.LastName = user.LastName;
        existingUser.Email = user.Email;
        existingUser.Password = user.Password;

        await _context.SaveChangesAsync(); // Save changes to database

        return existingUser;
    }
    catch (Exception ex)
    {
        // Log exception or handle as needed
        throw new Exception("Failed to update user", ex);
    }
}


        public async Task<User> DeleteAsync(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                    return null;

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                return user;
            }
            catch (Exception ex)
            {
                // Log the exception or handle as needed
                throw new Exception("Failed to delete user", ex);
            }
        }
    }
}
