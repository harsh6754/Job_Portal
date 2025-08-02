using Microsoft.AspNetCore.Mvc;

namespace MVC.Controllers
{
    public class UserDashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Applications(){
            return View();
        }

        
        public IActionResult Meeting(){
            return View();
        }
        public IActionResult savedjobs()
        {
            return View();
        }
    }
} 