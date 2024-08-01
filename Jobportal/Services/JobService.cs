//using JobPortal.Data;
//using JobPortal.Models;
//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;

//namespace JobPortal.Services
//{
//    public class JobService
//    {
//        private readonly ApplicationDbContext _context;

//        public JobService(ApplicationDbContext context)
//        {
//            _context = context ?? throw new ArgumentNullException(nameof(context));
//        }

//        public IEnumerable<Job> GetAllJobs()
//        {
//            return _context.Jobs.ToList(); // Materialize the query to execute immediately
//        }

//        public async Task<Job> GetJobByIdAsync(int id)
//        {
//            return await _context.Jobs.FindAsync(id);
//        }

//        public async Task<Job> CreateJobAsync(Job job)
//        {
//            try
//            {
//                _context.Jobs.Add(job);
//                await _context.SaveChangesAsync();
//                return job;
//            }
//            catch (Exception ex)
//            {
//                // Log exception or handle as needed
//                throw new Exception("Failed to create job", ex);
//            }
//        }

//        public async Task<Job> UpdateJobAsync(int id, Job job)
//        {
//            try
//            {
//                var existingJob = await _context.Jobs.FindAsync(id);
//                if (existingJob == null)
//                    return null;

//                existingJob.Title = job.Title;
//                existingJob.Description = job.Description;
//                existingJob.Location = job.Location;
//                existingJob.PostedDate = job.PostedDate;

//                await _context.SaveChangesAsync();
//                return existingJob;
//            }
//            catch (Exception ex)
//            {
//                // Log exception or handle as needed
//                throw new Exception("Failed to update job", ex);
//            }
//        }

//        public async Task<Job> DeleteJobAsync(int id)
//        {
//            try
//            {
//                var job = await _context.Jobs.FindAsync(id);
//                if (job == null)
//                    return null;

//                _context.Jobs.Remove(job);
//                await _context.SaveChangesAsync();
//                return job;
//            }
//            catch (Exception ex)
//            {
//                // Log exception or handle as needed
//                throw new Exception("Failed to delete job", ex);
//            }
//        }
//    }
//}
using JobPortal.Data; // Ensure this namespace is correct
using JobPortal.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JobPortal.Services
{
    public class JobService
    {
        private readonly ApplicationDbContext _context;

        public JobService(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Job>> GetAllJobsAsync()
        {
            return await _context.Jobs.ToListAsync(); // Asynchronous query execution
        }

        public IEnumerable<Job> GetAllJobs()
        {
            return _context.Jobs.ToList(); // Synchronous call if needed
        }

        public async Task<Job> GetJobByIdAsync(int id)
        {
            return await _context.Jobs.FindAsync(id);
        }

        //public async Task<Job> CreateJobAsync(Job job)
        //{
        //    try
        //    {
        //        _context.Jobs.Add(job);
        //        await _context.SaveChangesAsync();
        //        return job;
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log exception or handle as needed
        //        throw new Exception("Failed to create job", ex);
        //    }
        //}
        public async Task<Job> CreateJobAsync(Job job)
        {
            if (job.CompanyId <= 0)
            {
                throw new ArgumentException("Invalid company ID");
            }

            try
            {
                _context.Jobs.Add(job);
                await _context.SaveChangesAsync();
                return job;
            }
            catch (Exception ex)
            {
                // Log exception or handle as needed
                throw new Exception("Failed to create job", ex);
            }
        }


        public async Task<Job> UpdateJobAsync(int id, Job job)
        {
            try
            {
                var existingJob = await _context.Jobs.FindAsync(id);
                if (existingJob == null)
                    return null;

                existingJob.Title = job.Title;
                existingJob.Description = job.Description;
                existingJob.Location = job.Location;
                existingJob.PostedDate = job.PostedDate;

                await _context.SaveChangesAsync();
                return existingJob;
            }
            catch (Exception ex)
            {
                // Log exception or handle as needed
                throw new Exception("Failed to update job", ex);
            }
        }

        public async Task<Job> DeleteJobAsync(int id)
        {
            try
            {
                var job = await _context.Jobs.FindAsync(id);
                if (job == null)
                    return null;

                _context.Jobs.Remove(job);
                await _context.SaveChangesAsync();
                return job;
            }
            catch (Exception ex)
            {
                // Log exception or handle as needed
                throw new Exception("Failed to delete job", ex);
            }
        }
    }
}
