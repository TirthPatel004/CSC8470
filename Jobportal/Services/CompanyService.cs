
using JobPortal.Data;
using JobPortal.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JobPortal.Services
{
    public class CompanyService
    {
        private readonly ApplicationDbContext _context;

        public CompanyService(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // Method to get all companies
        public IEnumerable<Company> GetAllCompanies()
        {
            return _context.Companies.ToList();
        }

        // Async method to get all companies
        public async Task<IEnumerable<Company>> GetAllCompaniesAsync()
        {
            return await _context.Companies.ToListAsync();
        }

        // Method to get a company by ID
        public Company GetCompanyById(int id)
        {
            return _context.Companies.FirstOrDefault(c => c.Id == id);
        }

        // Async method to get a company by ID
        public async Task<Company> GetCompanyByIdAsync(int id)
        {
            return await _context.Companies.FindAsync(id);
        }

        // Async method to create a new company
        public async Task<Company> CreateCompanyAsync(Company company)
        {
            try
            {
                _context.Companies.Add(company);
                await _context.SaveChangesAsync();
                return company;
            }
            catch (Exception ex)
            {
                // Log exception or handle as needed
                throw new Exception("Failed to create company", ex);
            }
        }

        // Async method to update a company
        public async Task<Company> UpdateCompanyAsync(int id, Company company)
        {
            try
            {
                var existingCompany = await _context.Companies.FindAsync(id);
                if (existingCompany == null)
                    return null;

                existingCompany.Name = company.Name;
                existingCompany.Address = company.Address;
                existingCompany.Website = company.Website;
                existingCompany.Password = company.Password; // Ensure to hash this in a real-world scenario

                await _context.SaveChangesAsync();
                return existingCompany;
            }
            catch (Exception ex)
            {
                // Log exception or handle as needed
                throw new Exception("Failed to update company", ex);
            }
        }

        // Async method to delete a company
        public async Task<Company> DeleteCompanyAsync(int id)
        {
            try
            {
                var company = await _context.Companies.FindAsync(id);
                if (company == null)
                    return null;

                _context.Companies.Remove(company);
                await _context.SaveChangesAsync();
                return company;
            }
            catch (Exception ex)
            {
                // Log exception or handle as needed
                throw new Exception("Failed to delete company", ex);
            }
        }

        // Method to authenticate a company
        public Company Authenticate(string email, string password)
        {
            // Example: Authenticate logic based on email and password
            return _context.Companies.FirstOrDefault(c => c.Email == email && c.Password == password);
        }

        // Async method to get jobs by company ID
        public async Task<IEnumerable<Job>> GetJobsByCompanyIdAsync(int companyId)
        {
            return await _context.Jobs.Where(j => j.CompanyId == companyId).ToListAsync();
        }

        // Async method to get applications by company ID
        public async Task<IEnumerable<Application>> GetApplicationsByCompanyIdAsync(int companyId)
        {
            return await _context.Applications.Where(a => a.CompanyId == companyId).ToListAsync();
        }

        // Async method to get all users
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }
    }
}
