using GymProje.Data.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GymProje.WebUI.Controllers
{
    public class TrainerController : Controller
    {
        private readonly AppDbContext _context;

        public TrainerController(AppDbContext context)
        {
            _context = context;
        }

        // FİLTRELEME MANTIĞI BURADA
        public async Task<IActionResult> Index(int? trainerId, string specialty)
        {
            // 1. Veritabanındaki sorguyu başlatıyoruz (Henüz çekmedik)
            var query = _context.Trainers.Include(t => t.Gym).AsQueryable();

            // 2. Filtreler Dolu mu? Kontrol Et

            // Eğer kullanıcı "Uzmanlık" seçtiyse
            if (!string.IsNullOrEmpty(specialty))
            {
                query = query.Where(t => t.Specialty == specialty);
            }

            // Eğer kullanıcı spesifik bir "Hoca" seçtiyse
            if (trainerId.HasValue)
            {
                query = query.Where(t => t.Id == trainerId);
            }

            // 3. Dropdown (Seçim Kutusu) Verilerini Hazırla

            // Tüm Hocalar Listesi
            ViewBag.Trainers = new SelectList(await _context.Trainers.ToListAsync(), "Id", "FullName", trainerId);

            // Tüm Uzmanlıklar Listesi (Distinct ile tekrar edenleri eziyoruz)
            var specialties = await _context.Trainers
                                            .Select(t => t.Specialty)
                                            .Distinct()
                                            .ToListAsync();

            ViewBag.Specialties = new SelectList(specialties, specialty); // Seçili olan kalsın diye 'specialty' parametresi verdik

            // 4. Sonuçları Getir
            var result = await query.ToListAsync();
            return View(result);
        }
    }
}