using GymProje.Data.Context;
using GymProje.Entity.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymProje.WebUI.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AppointmentController : Controller
    {
        private readonly AppDbContext _context;

        public AppointmentController(AppDbContext context)
        {
            _context = context;
        }

        // 1. TÜM RANDEVULARI LİSTELE
        public async Task<IActionResult> Index()
        {
            var appointments = await _context.Appointments
                .Include(a => a.AppUser)  // Üye adını görmek için
                .Include(a => a.Trainer)  // Hoca adını görmek için
                .Include(a => a.GymService) // Hangi hizmet olduğunu görmek için
                .ThenInclude(gs => gs.Service) // Hizmetin adını almak için
                .OrderByDescending(x => x.CreatedDate) // En yeniden eskiye
                .ToListAsync();

            return View(appointments);
        }

        // 2. ONAYLA (Approve)
        public async Task<IActionResult> Approve(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                appointment.Status = AppointmentStatus.Confirmed; // Durumu ONAYLANDI yap
                await _context.SaveChangesAsync();
                TempData["Success"] = "Randevu onaylandı.";
            }
            return RedirectToAction(nameof(Index));
        }

        // 3. İPTAL ET / REDDET (Cancel)
        public async Task<IActionResult> Cancel(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                appointment.Status = AppointmentStatus.Cancelled; // Durumu İPTAL yap
                await _context.SaveChangesAsync();
                TempData["Error"] = "Randevu iptal edildi.";
            }
            return RedirectToAction(nameof(Index));
        }

        // 4. SİL (Delete) - Veritabanından tamamen silmek isterse
        public async Task<IActionResult> Delete(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment != null)
            {
                _context.Appointments.Remove(appointment);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Randevu kaydı silindi.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}