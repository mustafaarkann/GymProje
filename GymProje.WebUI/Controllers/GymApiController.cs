using GymProje.Data.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymProje.WebUI.Controllers
{
    [Route("api/[controller]")] // Adres: /api/GymApi
    [ApiController]
    public class GymApiController : ControllerBase
    {
        private readonly AppDbContext _context;

        public GymApiController(AppDbContext context)
        {
            _context = context;
        }

        // 1. TÜM ANTRENÖRLERİ GETİR (JSON)
        // İstek: GET /api/GymApi/trainers
        [HttpGet("trainers")]
        public async Task<IActionResult> GetTrainers()
        {
            // DİKKAT: Entity'leri direkt döndürürsek "Döngü Hatası" (Loop) alırız.
            // Çünkü Trainer -> Gym -> Trainer -> Gym... sonsuza gider.
            // Bu yüzden "Select" ile yeni, temiz bir obje (DTO) oluşturuyoruz.

            var trainers = await _context.Trainers
                .Include(t => t.Gym)
                .Select(t => new
                {
                    Id = t.Id,
                    AdSoyad = t.FullName,
                    Uzmanlik = t.Specialty,
                    Salon = t.Gym.Name,
                    Resim = t.ImageUrl
                })
                .ToListAsync();

            return Ok(trainers); // 200 OK ve JSON verisi döner
        }

        // 2. BELİRLİ BİR TARİHTEKİ RANDEVULARI GETİR (Filtreleme - LINQ)
        // İstek: GET /api/GymApi/appointments?date=2025-11-27
        [HttpGet("appointments")]
        public async Task<IActionResult> GetAppointments(DateTime? date)
        {
            if (date == null)
                return BadRequest("Lütfen bir tarih giriniz. Örn: ?date=2025-11-27");

            // Ödev İsteri: LINQ sorguları ile filtreleme [cite: 25]
            var appointments = await _context.Appointments
                .Include(a => a.Trainer)
                .Include(a => a.AppUser)
                .Where(a => a.AppointmentDate.Date == date.Value.Date) // Filtreleme
                .Select(a => new
                {
                    Tarih = a.AppointmentDate.ToString("dd.MM.yyyy"),
                    Saat = a.StartTime.ToString(@"hh\:mm") + " - " + a.EndTime.ToString(@"hh\:mm"),
                    Hoca = a.Trainer.FullName,
                    Ogrenci = a.AppUser.FirstName + " " + a.AppUser.LastName,
                    Durum = a.Status.ToString()
                })
                .ToListAsync();

            return Ok(appointments);
        }
    }
}