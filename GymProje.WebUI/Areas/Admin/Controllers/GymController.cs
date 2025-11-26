using GymProje.Data.Context;
using GymProje.Entity.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymProje.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")] // Admin bölgesindeyiz
    [Authorize(Roles = "Admin")] // Sadece Patron girebilir
    public class GymController : Controller
    {
        private readonly AppDbContext _context;

        public GymController(AppDbContext context)
        {
            _context = context;
        }

        // 1. LİSTELEME SAYFASI (INDEX)
        public async Task<IActionResult> Index()
        {
            // Veritabanındaki tüm salonları çekip listeye atıyoruz.
            var gyms = await _context.Gyms.ToListAsync();
            return View(gyms);
        }

        // 2. EKLEME SAYFASI (GET) - Formu Göster
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // 3. EKLEME İŞLEMİ (POST) - Veriyi Kaydet
        [HttpPost]
        public async Task<IActionResult> Create(Gym gym)
        {
            if (ModelState.IsValid)
            {
                _context.Gyms.Add(gym);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Yeni salon başarıyla eklendi!";
                return RedirectToAction(nameof(Index));
            }

            // Hata varsa formu geri göster (Hatalarıyla birlikte)
            return View(gym);
        }

        // 4. SİLME İŞLEMİ
        public async Task<IActionResult> Delete(int id)
        {
            var gym = await _context.Gyms.FindAsync(id);
            if (gym != null)
            {
                _context.Gyms.Remove(gym);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Salon silindi.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}