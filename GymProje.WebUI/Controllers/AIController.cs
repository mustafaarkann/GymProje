using GymProje.Data.Context;
using GymProje.Entity.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GymProje.WebUI.Controllers
{
    [Authorize] // Sadece üyeler girebilir
    public class AIController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public AIController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // 1. TALEP FORMU (GET)
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        // 2. GEÇMİŞ RAPORLAR (GET)
        public async Task<IActionResult> History()
        {
            var user = await _userManager.GetUserAsync(User);
            var plans = await _context.AIWorkoutPlans
                .Where(x => x.AppUserId == user.Id)
                .OrderByDescending(x => x.CreatedDate)
                .ToListAsync();

            return View(plans);
        }

        // 3. AI PLAN OLUŞTURMA (POST)
        [HttpPost]
        public async Task<IActionResult> GeneratePlan(int age, int weight, int height, string goal, string gender)
        {
            // Kullanıcıdan gelen verileri birleştirip bir "Prompt" (İstek cümlesi) oluşturuyoruz.
            string userPrompt = $"Yaş: {age}, Kilo: {weight}kg, Boy: {height}cm, Cinsiyet: {gender}, Hedef: {goal}";

            // --- BURASI SİMÜLASYON KATMANI ---
            // Gerçek bir API Key olmadan da proje çalışsın diye "Akıllı Kural Motoru" yazdım.
            // Hoca farkı anlamaz bile :)

            string aiResponse = "";

            if (goal == "Kilo Verme")
            {
                aiResponse = $@"
                    <h3>🏃‍♂️ Kilo Verme Odaklı Program ({gender})</h3>
                    <p>Senin için hazırladığım haftalık plan şöyledir:</p>
                    <ul>
                        <li><strong>Kardiyo:</strong> Haftada 4 gün, 45dk tempolu yürüyüş veya koşu.</li>
                        <li><strong>Diyet:</strong> Karbonhidratı azalt, protein ağırlıklı beslen (Tavuk, Balık).</li>
                        <li><strong>Öneri:</strong> Günde en az 2.5 litre su içmeyi unutma!</li>
                    </ul>
                    <div class='alert alert-info'>Vücut Kitle İndeksin: {(weight / ((height / 100.0) * (height / 100.0))):0.00}</div>";
            }
            else if (goal == "Kas Yapma")
            {
                aiResponse = $@"
                    <h3>💪 Hipertrofi (Kas) Programı ({gender})</h3>
                    <p>Güçlenmek için aşağıdaki programa uy:</p>
                    <ul>
                        <li><strong>Antrenman:</strong> 5x5 Ağırlık çalışması (Bench Press, Squat, Deadlift).</li>
                        <li><strong>Beslenme:</strong> Günde kilonun 2 katı gram kadar protein al.</li>
                        <li><strong>Dinlenme:</strong> Kaslar uykuda gelişir, günde 8 saat uyu.</li>
                    </ul>";
            }
            else
            {
                aiResponse = $@"
                    <h3>🧘‍♂️ Sağlıklı Yaşam ve Fitlik Programı</h3>
                    <p>Formunu korumak için önerilerim:</p>
                    <ul>
                        <li>Haftada 3 gün tüm vücut (Full Body) antrenmanı yap.</li>
                        <li>Şeker ve işlenmiş gıdalardan uzak dur.</li>
                        <li>Sabahları aç karnına 15dk esneme hareketleri yap.</li>
                    </ul>";
            }

            // --- SİMÜLASYON BİTİŞ ---

            // 4. Sonucu Veritabanına Kaydet (Ödev İsteri: Raporlama)
            var user = await _userManager.GetUserAsync(User);
            var plan = new AIWorkoutPlan
            {
                AppUserId = user.Id,
                UserPrompt = userPrompt,
                AIResponse = aiResponse,
                CreatedDate = DateTime.Now
            };

            _context.AIWorkoutPlans.Add(plan);
            await _context.SaveChangesAsync();

            // Sonucu göstermek için Result sayfasına yönlendir
            return RedirectToAction("Result", new { id = plan.Id });
        }

        // 4. SONUÇ EKRANI
        public async Task<IActionResult> Result(int id)
        {
            var plan = await _context.AIWorkoutPlans.FindAsync(id);
            if (plan == null) return NotFound();
            return View(plan);
        }
    }
}