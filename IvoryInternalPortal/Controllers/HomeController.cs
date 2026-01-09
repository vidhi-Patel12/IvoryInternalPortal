using Microsoft.AspNetCore.Mvc;

namespace IvoryInternalPortal.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Logout()
        {
            // Remove cookies
            Response.Cookies.Delete("RoleId");
            Response.Cookies.Delete("RoleName");
            Response.Cookies.Delete("Email");

            // Redirect to Login page
            return RedirectToAction("Index", "Home");
        }
    }
}
