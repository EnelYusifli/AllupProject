using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AllupProject.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
