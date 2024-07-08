﻿using System.ComponentModel.DataAnnotations;

namespace JobPortal.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }

    }

    public class Job
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public DateTime PostedDate { get; set; }

        // Foreign key for Company
        public int CompanyId { get; set; }
       // public Company Company { get; set; }

        // Navigation properties
       // public ICollection<Application> Applications { get; set; }
    }

    public class Application
    {
        public int Id { get; set; }

        // Foreign key for Job
        public int JobId { get; set; }
       // public Job Job { get; set; }

        // Foreign key for User
        public int UserId { get; set; }
        //public User User { get; set; }

        public DateTime AppliedDate { get; set; }
        public string Resume { get; set; }
    }

    public class Company
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Website { get; set; }
        public string Password { get; set; } // Added Password field
        public string Email { get; set; }

        // Navigation properties
        // public ICollection<Job> Jobs { get; set;
        // }

    
    }
    public class LoginModel
    {
        [Required(ErrorMessage = "The email field is required.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "The password field is required.")]
        public string Password { get; set; }
    }
}