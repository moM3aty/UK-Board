using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UkBoard.Data;
using UkBoard.Models;
using System.Diagnostics;

namespace UkBoard.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Agencies()
        {
            return View();
        }

        public IActionResult Blog()
        {
            return View();
        }

        public IActionResult Certificates()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> SearchCertificate(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return Json(new { success = false, message = "Please enter an ID." });
            }

            var certificate = await _context.Certificates
                                      .FirstOrDefaultAsync(c => c.CertificateId == id);

            if (certificate != null)
            {
                return Json(new { success = true, imagePath = certificate.ImagePath });
            }
            else
            {
                return Json(new { success = false, message = "No certificate found for this ID." });
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
