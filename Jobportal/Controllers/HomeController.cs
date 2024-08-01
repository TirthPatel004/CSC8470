//using Jobportal.Models;
//using Microsoft.AspNetCore.Mvc;
//using System.Diagnostics;

//namespace Jobportal.Controllers
//{
//    public class HomeController : Controller
//    {
//        private readonly ILogger<HomeController> _logger;

//        public HomeController(ILogger<HomeController> logger)
//        {
//            _logger = logger;
//        }

//        public IActionResult Index()
//        {
//            return View();
//        }



//        [HttpGet("Jobs")]
//        public IActionResult Job()
//        {
//            return View();
//        }
//        public IActionResult Privacy()
//        {
//            return View();
//        }

//        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
//        public IActionResult Error()
//        {
//            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
//        }
//    }
//}
using Jobportal.Models;
using JobPortal.Models;
using JobPortal.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Jobportal.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly JobService _jobService;
        private readonly ApplicationService _applicationService;

        public HomeController(ILogger<HomeController> logger, JobService jobService, ApplicationService applicationService)
        {
            _logger = logger;
            _jobService = jobService;
            _applicationService = applicationService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("Jobs")]
        public IActionResult Job()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        //[HttpGet("Company/CompanyDashboard")]
        //public async Task<IActionResult> CompanyDashboard()
        //{
        //    try
        //    {
        //        var jobs = await _jobService.GetAllJobsAsync();
        //        var applications = await _applicationService.GetAllApplicationsAsync();

        //        var model = new CompanyDashboardModel
        //        {
        //            Jobs = jobs,
        //            Applications = applications
        //        };

        //        return View("CompanyDashboard", model);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "An error occurred while loading the Company Dashboard.");
        //        return View("Error");
        //    }
        //}
        [HttpGet("Company/CompanyDashboard")]
        public async Task<IActionResult> CompanyDashboard()
        {
            try
            {
                var jobs = await _jobService.GetAllJobsAsync();
                var applications = await _applicationService.GetAllApplicationsAsync();

                var model = new CompanyDashboardModel
                {
                    Jobs = jobs ?? new List<Job>(),
                    Applications = applications ?? new List<Application>()
                };

                ViewData["Title"] = "Company Dashboard";
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while loading the Company Dashboard.");
                return View("Error");
            }
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
