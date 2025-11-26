using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using GymProje.Data.Context;
using GymProje.Entity.Entities;

var builder = WebApplication.CreateBuilder(args);

// 1. Veritabaný Baðlantýsýný Oku
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 2. DbContext'i Sisteme Tanýt (SQL Server Kullanarak)
builder.Services.AddDbContext<GymProje.Data.Context.AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// 3. Identity (Kullanýcý Giriþ Sistemi) Ayarlarý
builder.Services.AddIdentity<GymProje.Entity.Entities.AppUser, Microsoft.AspNetCore.Identity.IdentityRole>(options =>
{
    // Ödev için Gevþek Þifre Kurallarý (Admin þifresi 'sau' olabilsin diye)
    options.Password.RequireDigit = false; // Sayý zorunluluðu yok
    options.Password.RequireLowercase = false; // Küçük harf zorunluluðu yok
    options.Password.RequireUppercase = false; // Büyük harf zorunluluðu yok
    options.Password.RequireNonAlphanumeric = false; // Sembol (!, @) zorunluluðu yok
    options.Password.RequiredLength = 3; // En az 3 karakter olsun ('sau' 3 karakter)
})
.AddEntityFrameworkStores<GymProje.Data.Context.AppDbContext>()
.AddDefaultTokenProviders();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Area (Bölge) Rotalarý için tanýmlama
// Bu sayede /Admin/Dashboard/Index gibi adreslere gidebileceðiz.
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// --- DATA SEEDING (Otomatik Veri Ekleme) BAÞLANGIÇ ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var userManager = services.GetRequiredService<UserManager<AppUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        // Yazdýðýmýz Seed metodunu çaðýrýyoruz
        // 'Wait' diyerek bitmesini bekliyoruz (Async olduðu için)
        GymProje.Data.DbInitializer.Seed(userManager, roleManager).Wait();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Admin kullanýcýsý oluþturulurken hata çýktý.");
    }
}
// --- DATA SEEDING BÝTÝÞ ---



app.Run();
