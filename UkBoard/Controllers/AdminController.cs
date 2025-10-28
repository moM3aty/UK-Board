using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UkBoard.Data;
using UkBoard.Models;
using System.Threading.Tasks;

namespace UkBoard.Controllers
{
    [Authorize(Roles = "Admin")] // تأمين هذا الكونترولر بالكامل للمدير فقط
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AdminController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Admin
        // GET: Admin?searchString=...
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

        // GET: Admin/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Create
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

        // GET: Admin/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var certificate = await _context.Certificates.FindAsync(id);
            if (certificate == null)
            {
                return NotFound();
            }

            // تحويل الموديل الأساسي إلى موديل العرض
            CertificateViewModel model = new CertificateViewModel
            {
                Id = certificate.Id,
                CertificateId = certificate.CertificateId,
                ExistingImagePath = certificate.ImagePath
            };

            return View(model);
        }

        // POST: Admin/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CertificateViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var certificateToUpdate = await _context.Certificates.FindAsync(id);
                if (certificateToUpdate == null)
                {
                    return NotFound();
                }

                certificateToUpdate.CertificateId = model.CertificateId;

                // التحقق إذا تم رفع صورة جديدة
                if (model.CertificateImage != null)
                {
                    // 1. حذف الصورة القديمة
                    if (!string.IsNullOrEmpty(certificateToUpdate.ImagePath))
                    {
                        DeleteFile(certificateToUpdate.ImagePath);
                    }
                    // 2. رفع الصورة الجديدة
                    certificateToUpdate.ImagePath = await UploadFile(model.CertificateImage);
                }

                try
                {
                    _context.Update(certificateToUpdate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Certificates.Any(e => e.Id == model.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // POST: Admin/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var certificate = await _context.Certificates.FindAsync(id);
            if (certificate == null)
            {
                return NotFound();
            }

            // 1. حذف ملف الصورة من السيرفر
            if (!string.IsNullOrEmpty(certificate.ImagePath))
            {
                DeleteFile(certificate.ImagePath);
            }

            // 2. حذف السجل من قاعدة البيانات
            _context.Certificates.Remove(certificate);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // دالة مساعدة لرفع الملفات
        private async Task<string> UploadFile(IFormFile file)
        {
            string uniqueFileName = string.Empty;
            if (file != null)
            {
                string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
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
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }
    }
}

