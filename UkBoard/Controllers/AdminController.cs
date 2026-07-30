using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UkBoard.Data;
using UkBoard.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using System;
using System.Linq;

namespace UkBoard.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AdminController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // ==========================================
        // Certificates Management (Existing)
        // ==========================================
        public async Task<IActionResult> Index(string searchString)
        {
            ViewData["CurrentFilter"] = searchString;

            var certificates = from c in _context.Certificates
                               select c;

            if (!String.IsNullOrEmpty(searchString))
            {
                certificates = certificates.Where(s => s.CertificateId.Contains(searchString));
            }

            return View(await certificates.ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CertificateViewModel model)
        {
            if (model.CertificateImage == null)
            {
                ModelState.AddModelError("CertificateImage", "Please select a certificate image.");
            }

            if (ModelState.IsValid)
            {
                string uniqueFileName = await UploadFile(model.CertificateImage);

                Certificate certificate = new Certificate
                {
                    CertificateId = model.CertificateId,
                    ImagePath = uniqueFileName
                };

                _context.Add(certificate);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var certificate = await _context.Certificates.FindAsync(id);
            if (certificate == null) return NotFound();

            CertificateViewModel model = new CertificateViewModel
            {
                Id = certificate.Id,
                CertificateId = certificate.CertificateId,
                ExistingImagePath = certificate.ImagePath
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CertificateViewModel model)
        {
            if (id != model.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var certificateToUpdate = await _context.Certificates.FindAsync(id);
                if (certificateToUpdate == null) return NotFound();

                certificateToUpdate.CertificateId = model.CertificateId;

                if (model.CertificateImage != null)
                {
                    if (!string.IsNullOrEmpty(certificateToUpdate.ImagePath))
                    {
                        DeleteFile(certificateToUpdate.ImagePath);
                    }
                    certificateToUpdate.ImagePath = await UploadFile(model.CertificateImage);
                }

                try
                {
                    _context.Update(certificateToUpdate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Certificates.Any(e => e.Id == model.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var certificate = await _context.Certificates.FindAsync(id);
            if (certificate == null) return NotFound();

            if (!string.IsNullOrEmpty(certificate.ImagePath))
            {
                DeleteFile(certificate.ImagePath);
            }

            _context.Certificates.Remove(certificate);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // ==========================================
        // Student Registrations Management (New)
        // ==========================================

        public async Task<IActionResult> StudentRegistrations(string searchString)
        {
            ViewData["CurrentFilter"] = searchString;

            var registrations = from r in _context.StudentRegistrations select r;

            if (!String.IsNullOrEmpty(searchString))
            {
                registrations = registrations.Where(s =>
                    s.FullName.Contains(searchString) ||
                    s.PhoneNumber.Contains(searchString) ||
                    s.Major.Contains(searchString));
            }

            // الترتيب ليكون الأحدث أولاً
            return View(await registrations.OrderByDescending(r => r.RegistrationDate).ToListAsync());
        }

        public async Task<IActionResult> ViewRegistration(int? id)
        {
            if (id == null) return NotFound();

            var registration = await _context.StudentRegistrations.FindAsync(id);
            if (registration == null) return NotFound();

            // جعل الحالة "تمت القراءة" بمجرد الفتح
            if (!registration.IsRead)
            {
                registration.IsRead = true;
                _context.Update(registration);
                await _context.SaveChangesAsync();
            }

            return View(registration);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRegistration(int id)
        {
            var registration = await _context.StudentRegistrations.FindAsync(id);
            if (registration == null) return NotFound();

            // حذف الملفات المرتبطة بالطالب
            DeleteFile(registration.QualificationImagePath);
            DeleteFile(registration.IdentityImagePath);
            DeleteFile(registration.PersonalPhotoPath);

            _context.StudentRegistrations.Remove(registration);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(StudentRegistrations));
        }

        // ==========================================
        // Helper Methods
        // ==========================================
        private async Task<string> UploadFile(IFormFile file)
        {
            string uniqueFileName = string.Empty;
            if (file != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }
            }
            return Path.Combine("uploads", uniqueFileName).Replace("\\", "/");
        }

        private void DeleteFile(string imagePath)
        {
            if (string.IsNullOrEmpty(imagePath)) return;
            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, imagePath);
            if (System.IO.File.Exists(filePath)) System.IO.File.Delete(filePath);
        }
    }
}