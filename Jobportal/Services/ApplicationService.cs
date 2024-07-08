using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JobPortal.Data;
using JobPortal.Models;

namespace JobPortal.Services
{
    public class ApplicationService
    {
        private readonly ApplicationDbContext _context;

        public ApplicationService(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<List<Application>> GetAllApplicationsAsync()
        {
            return await _context.Applications.ToListAsync();
        }

        public async Task<Application> GetApplicationByIdAsync(int id)
        {
            return await _context.Applications.FindAsync(id);
        }

        public async Task<Application> CreateApplicationAsync(Application application)
        {
            _context.Applications.Add(application);
            await _context.SaveChangesAsync();
            return application;
        }

        public async Task<Application> UpdateApplicationAsync(int id, Application application)
        {
            var existingApplication = await _context.Applications.FindAsync(id);
            if (existingApplication == null)
                return null;

            existingApplication.JobId = application.JobId;
            existingApplication.UserId = application.UserId;
            existingApplication.AppliedDate = application.AppliedDate;
            existingApplication.Resume = application.Resume;

            await _context.SaveChangesAsync();
            return existingApplication;
        }

        public async Task<Application> DeleteApplicationAsync(int id)
        {
            var application = await _context.Applications.FindAsync(id);
            if (application == null)
                return null;

            _context.Applications.Remove(application);
            await _context.SaveChangesAsync();
            return application;
        }
    }
}
