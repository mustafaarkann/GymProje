using GymProje.Entity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GymProje.WebUI.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;

        // Dependency Injection ile SignInManager'ı çağırıyoruz.
        // Bu sınıf şifre kontrolü ve cookie (çerez) işlemlerini yapar.
        public AccountController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager; 
        }

        // 1. LOGIN SAYFASINI GÖSTER (GET)
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // 2. LOGIN İŞLEMİNİ YAP (POST)
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            // Kullanıcıdan gelen verileri kontrol et
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Lütfen e-posta ve şifre giriniz.";
                return View();
            }

            // Identity kütüphanesi ile giriş yapmayı dene
            // false, false parametreleri: "Beni Hatırla" kapalı, "Hesap Kilitleme" kapalı.
            var result = await _signInManager.PasswordSignInAsync(email, password, false, false);

            if (result.Succeeded)
            {
                // Giriş başarılıysa Ana Sayfaya yönlendir
                // (Admin ise kendisi /Admin'e gitmek isteyecektir)
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "E-posta veya şifre hatalı!";
            return View();
        }

        // 3. ÇIKIŞ YAP (LOGOUT)
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync(); // Çerezi siler
            return RedirectToAction("Index", "Home");
        }

        // --- BURADAN AŞAĞISINI EKLE ---

        // 4. KAYIT SAYFASI (GET)
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // 5. KAYIT İŞLEMİ (POST)
        [HttpPost]
        public async Task<IActionResult> Register(string firstName, string lastName, string email, string password)
        {
            // 1. Basit Validasyonlar
            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) ||
                string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ViewBag.Error = "Lütfen tüm alanları doldurunuz.";
                return View();
            }

            // 2. Yeni Kullanıcı Nesnesi Oluştur
            var newUser = new AppUser
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                UserName = email, // Kullanıcı adı e-posta olsun
                EmailConfirmed = true, // Onaylı varsayalım
                BirthDate = DateTime.Now.AddYears(-20) // Varsayılan yaş (İleride formdan alabiliriz)
            };

            // 3. Identity Kütüphanesi ile Kaydet
            var result = await _userManager.CreateAsync(newUser, password);

            if (result.Succeeded)
            {
                // 4. BAŞARILIYSA: Otomatik "Member" rolü ata
                await _userManager.AddToRoleAsync(newUser, "Member");

                // 5. Giriş Yaptır ve Ana Sayfaya Gönder
                await _signInManager.SignInAsync(newUser, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }

            // Hata varsa (Örn: Şifre çok basit, E-posta kayıtlı)
            foreach (var error in result.Errors)
            {
                // Hataları tek bir mesajda birleştirip gösterelim
                ViewBag.Error += error.Description + " ";
            }

            return View();
        }
    }
}