using GymProje.Data.Context;
using GymProje.Entity.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GymProje.WebUI.Controllers
{
    [Authorize] // Sadece giriş yapmış üyeler
    public class AppointmentController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public AppointmentController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // 1. RANDEVU FORMU (GET)
        [HttpGet]
        public async Task<IActionResult> Make(int trainerId)
        {
            var trainer = await _context.Trainers
                .Include(t => t.Gym)
                .FirstOrDefaultAsync(t => t.Id == trainerId);

            if (trainer == null) return NotFound();

            // Seçilen salonun hizmetlerini getir
            var services = await _context.GymServices
                .Include(gs => gs.Service)
                .Where(gs => gs.GymId == trainer.GymId)
                .Select(gs => new
                {
                    Id = gs.Id,
                    Name = gs.Service.Name + " (" + gs.Price + " TL - " + gs.DurationMinutes + " dk)"
                })
                .ToListAsync();

            ViewBag.Services = new SelectList(services, "Id", "Name");
            ViewBag.TrainerName = trainer.FullName;
            ViewBag.TrainerId = trainer.Id;
            ViewBag.GymName = trainer.Gym.Name;

            return View();
        }

        // 2. RANDEVU KAYDETME (POST)
        [HttpPost]
        public async Task<IActionResult> Make(int trainerId, int gymServiceId, DateTime date, TimeSpan time)
        {
            var user = await _userManager.GetUserAsync(User);
            var gymService = await _context.GymServices.FindAsync(gymServiceId);

            if (gymService == null) return NotFound();

            // Tarih Hesaplamaları
            DateTime startDateTime = date.Date + time;
            DateTime endDateTime = startDateTime.AddMinutes(gymService.DurationMinutes);

            // Geçmiş Kontrolü
            if (startDateTime < DateTime.Now)
            {
                ViewBag.Error = "Geçmiş bir tarihe randevu alamazsınız.";
                return await ReloadPage(trainerId);
            }

            // ÇAKIŞMA KONTROLÜ
            bool isConflict = await _context.Appointments.AnyAsync(a =>
                a.TrainerId == trainerId &&
                a.AppointmentDate == date.Date &&
                a.Status != AppointmentStatus.Cancelled &&
                (time < a.EndTime && a.StartTime < endDateTime.TimeOfDay)
            );

            if (isConflict)
            {
                ViewBag.Error = "Seçtiğiniz saatte antrenör dolu. Lütfen başka saat seçin.";
                return await ReloadPage(trainerId);
            }

            // Kayıt
            var appointment = new Appointment
            {
                AppUserId = user.Id,
                TrainerId = trainerId,
                GymServiceId = gymServiceId,
                AppointmentDate = date.Date,
                StartTime = time,
                EndTime = endDateTime.TimeOfDay,
                Status = AppointmentStatus.Pending, // Enum burada kullanılıyor
                CreatedDate = DateTime.Now
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Randevu talebiniz alındı! Onay bekleniyor.";
            return RedirectToAction("Index", "Home");
        }

        // 3. KULLANICININ KENDİ RANDEVULARI (GET)
        [HttpGet]
        public async Task<IActionResult> MyAppointments()
        {
            var user = await _userManager.GetUserAsync(User); // Giriş yapan kullanıcıyı bul

            var appointments = await _context.Appointments
                .Include(a => a.Trainer)                // Hoca bilgisi
                .Include(a => a.GymService)             // Hizmet bilgisi
                .ThenInclude(gs => gs.Service)          // Hizmet adı
                .Where(a => a.AppUserId == user.Id)     // SADECE BU KULLANICININ RANDEVULARI
                .OrderByDescending(a => a.AppointmentDate) // En yeni en üstte
                .ToListAsync();

            return View(appointments);
        }

        // Yardımcı Metod (Sayfa yenilendiğinde verileri tekrar doldurur)
        private async Task<IActionResult> ReloadPage(int trainerId)
        {
            var trainer = await _context.Trainers.Include(t => t.Gym).FirstOrDefaultAsync(t => t.Id == trainerId);
            var services = await _context.GymServices
                .Include(gs => gs.Service)
                .Where(gs => gs.GymId == trainer.GymId)
                .Select(gs => new { Id = gs.Id, Name = gs.Service.Name + " (" + gs.Price + " TL - " + gs.DurationMinutes + " dk)" })
                .ToListAsync();

            ViewBag.Services = new SelectList(services, "Id", "Name");
            ViewBag.TrainerName = trainer.FullName;
            ViewBag.TrainerId = trainer.Id;
            ViewBag.GymName = trainer.Gym.Name;
            return View("Make"); // Make.cshtml'i tekrar göster
        }
    }
}