using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MVC.Controllers
{
    public class CandidateJobController : Controller
    {
        private readonly ILogger<CandidateJobController> _logger;

        public CandidateJobController(ILogger<CandidateJobController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }

        public IActionResult ViewPostedJob()
        {
            return View();
        }

        public IActionResult JobDetails()
        {
            return View();
        }

        public IActionResult CompanyProfile(int companyId)
        {
            ViewBag.CompanyId = companyId;
            return View();
        }
    }
}