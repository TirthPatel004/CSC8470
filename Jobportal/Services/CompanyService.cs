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

        public IEnumerable<Company> GetAllCompanies()
        {
            return _context.Companies;
        }

        public Company GetCompanyById(int id)
        {
            return _context.Companies.FirstOrDefault(c => c.Id == id);
        }

        public async Task<Company> GetCompanyByIdAsync(int id)
        {
            return await _context.Companies.FindAsync(id);
        }

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

        public Company Authenticate(string email, string password)
        {
            // Example: Authenticate logic based on email and password
            return _context.Companies.FirstOrDefault(c => c.Email == email && c.Password == password);
        }
    }
}
