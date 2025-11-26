using GymProje.Data.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace GymProje.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Tüm antrenörleri ve çalýþtýklarý salon bilgisini çekiyoruz.
            var trainers = await _context.Trainers
                .Include(t => t.Gym)
                .ToListAsync();

            return View(trainers);
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}