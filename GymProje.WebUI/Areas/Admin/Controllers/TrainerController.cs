using GymProje.Data.Context;
using GymProje.Entity.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering; // SelectList için gerekli
using Microsoft.EntityFrameworkCore;

namespace GymProje.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class TrainerController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment; // Dosya kaydetmek için gerekli servis

        public TrainerController(AppDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        // 1. LİSTELEME
        public async Task<IActionResult> Index()
        {
            // Include(x => x.Gym) diyerek hocanın bağlı olduğu salonun adını da çekiyoruz (JOIN işlemi).
            var trainers = await _context.Trainers.Include(t => t.Gym).ToListAsync();
            return View(trainers);
        }

        // 2. EKLEME SAYFASI (GET)
        [HttpGet]
        public IActionResult Create()
        {
            // Veritabanındaki salonları çekip, bir "Seçim Listesi" (SelectList) hazırlıyoruz.
            // Value = Id (Arka planda tutulan), Text = Name (Ekranda görünen)
            ViewBag.Gyms = new SelectList(_context.Gyms, "Id", "Name");
            return View();
        }

        // 3. EKLEME İŞLEMİ (POST)
        [HttpPost]
        public async Task<IActionResult> Create(Trainer trainer, IFormFile? file)
        {
            // Validasyon kontrolü (Model geçerli mi?)
            // Resim yüklenmediyse de hata vermesin diye file kontrolünü ayrı yapıyoruz.

            // A. Dosya Yükleme İşlemi
            if (file != null)
            {
                // 1. Resmin uzantısını al (.jpg, .png)
                string wwwRootPath = _hostEnvironment.WebRootPath;
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                string path = Path.Combine(wwwRootPath, @"img\trainers", fileName);

                // 2. Resmi klasöre kopyala
                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                // 3. Veritabanına sadece dosya adını kaydet
                trainer.ImageUrl = fileName;
            }
            else
            {
                trainer.ImageUrl = "no-image.png"; // Resim yüklemezse varsayılan bir isim ata
            }

            // B. Veritabanı Kaydı
            _context.Trainers.Add(trainer);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Antrenör başarıyla eklendi.";
            return RedirectToAction(nameof(Index));
        }

        // 4. SİLME İŞLEMİ
        public async Task<IActionResult> Delete(int id)
        {
            var trainer = await _context.Trainers.FindAsync(id);
            if (trainer != null)
            {
                // İstersen burada System.IO.File.Delete ile resmi de klasörden silebilirsin.
                _context.Trainers.Remove(trainer);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Antrenör silindi.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}