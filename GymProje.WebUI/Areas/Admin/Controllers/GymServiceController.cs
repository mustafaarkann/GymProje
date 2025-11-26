using GymProje.Data.Context;
using GymProje.Entity.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GymProje.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class GymServiceController : Controller
    {
        private readonly AppDbContext _context;

        public GymServiceController(AppDbContext context)
        {
            _context = context;
        }

        // 1. LİSTELEME
        public async Task<IActionResult> Index()
        {
            // İlgili Salon ve Hizmet adlarını getirmek için Include şart
            var items = await _context.GymServices
                .Include(x => x.Gym)
                .Include(x => x.Service)
                .ToListAsync();
            return View(items);
        }

        // 2. EKLEME SAYFASI (GET)
        [HttpGet]
        public IActionResult Create()
        {
            // Sayfa ilk açıldığında dropdownları doldur
            ViewBag.Gyms = new SelectList(_context.Gyms, "Id", "Name");
            ViewBag.Services = new SelectList(_context.Services, "Id", "Name");
            return View();
        }

        // 3. EKLEME İŞLEMİ (POST)
        [HttpPost]
        public async Task<IActionResult> Create(GymService gymService)
        {
            // VALIDATION FIX:
            // Formdan Gym ve Service nesneleri gelmiyor, sadece ID'leri geliyor.
            // Bu yüzden "Gym alanı boş olamaz" hatasını bu satırlarla engelliyoruz.
            ModelState.Remove("Gym");
            ModelState.Remove("Service");

            // AYNI HİZMET KONTROLÜ:
            // Bu salonda bu hizmet daha önce eklenmiş mi?
            bool exists = await _context.GymServices.AnyAsync(x =>
                x.GymId == gymService.GymId && x.ServiceId == gymService.ServiceId);

            if (exists)
            {
                ViewBag.Error = "Bu hizmet bu salonda zaten tanımlı!";

                // Hata olduğu için sayfayı tekrar gösteriyoruz, dropdownları yeniden doldurmalıyız
                ViewBag.Gyms = new SelectList(_context.Gyms, "Id", "Name");
                ViewBag.Services = new SelectList(_context.Services, "Id", "Name");
                return View(gymService);
            }

            if (ModelState.IsValid)
            {
                _context.GymServices.Add(gymService);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Fiyatlandırma başarıyla eklendi.";
                return RedirectToAction(nameof(Index));
            }

            // Eğer başka bir validasyon hatası varsa (Örn: Fiyat negatifse) dropdownları doldurup geri dön
            ViewBag.Gyms = new SelectList(_context.Gyms, "Id", "Name");
            ViewBag.Services = new SelectList(_context.Services, "Id", "Name");

            return View(gymService);
        }

        // 4. SİLME İŞLEMİ
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _context.GymServices.FindAsync(id);
            if (item != null)
            {
                _context.GymServices.Remove(item);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Kayıt silindi.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}