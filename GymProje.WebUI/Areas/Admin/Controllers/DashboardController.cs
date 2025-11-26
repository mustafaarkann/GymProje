using GymProje.Data.Context;
using GymProje.Entity.Entities; // AppointmentStatus için
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymProje.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // 1. İSTATİSTİK KARTLARI İÇİN SAYILAR
            ViewBag.GymCount = await _context.Gyms.CountAsync();
            ViewBag.TrainerCount = await _context.Trainers.CountAsync();
            ViewBag.ServiceCount = await _context.Services.CountAsync();

            // Bekleyen (Onaylanmamış) Randevu Sayısı
            ViewBag.PendingAppointments = await _context.Appointments
                .Where(x => x.Status == AppointmentStatus.Pending)
                .CountAsync();

            // 2. SON 5 RANDEVU LİSTESİ (Tablo İçin)
            // Kim, Hangi Hocadan, Ne Zaman randevu almış?
            var recentAppointments = await _context.Appointments
                .Include(a => a.AppUser) // Üye ismini görmek için
                .Include(a => a.Trainer) // Hoca ismini görmek için
                .OrderByDescending(x => x.CreatedDate) // En yeniden eskiye
                .Take(5) // Sadece son 5 tanesi
                .ToListAsync();

            return View(recentAppointments);
        }
    }
}