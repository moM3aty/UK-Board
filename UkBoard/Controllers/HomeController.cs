using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UkBoard.Data;
using UkBoard.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using System;

namespace UkBoard.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
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

        // ==========================================
        // Registration Endpoints
        // ==========================================
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(StudentRegistrationViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // رفع الملفات والحصول على المسارات
                    string qualPath = await UploadRegistrationFile(model.QualificationImage, "qualifications");
                    string idPath = await UploadRegistrationFile(model.IdentityImage, "identities");
                    string photoPath = await UploadRegistrationFile(model.PersonalPhoto, "photos");

                    var registration = new StudentRegistration
                    {
                        FullName = model.FullName,
                        PhoneNumber = model.PhoneNumber,
                        Major = model.Major,
                        QualificationImagePath = qualPath,
                        IdentityImagePath = idPath,
                        PersonalPhotoPath = photoPath,
                        IsTransparencyCharterAgreed = model.IsTransparencyCharterAgreed,
                        RegistrationDate = DateTime.Now,
                        IsRead = false
                    };

                    _context.StudentRegistrations.Add(registration);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Your application has been submitted successfully!";
                    return RedirectToAction(nameof(Register));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while saving registration.");
                    ModelState.AddModelError("", "An error occurred while submitting your application. Please try again.");
                }
            }
            return View(model);
        }

        private async Task<string> UploadRegistrationFile(IFormFile file, string subFolder)
        {
            if (file == null || file.Length == 0) return null;

            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "registrations", subFolder);
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return Path.Combine("uploads", "registrations", subFolder, uniqueFileName).Replace("\\", "/");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}