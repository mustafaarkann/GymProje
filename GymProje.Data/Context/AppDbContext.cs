using GymProje.Entity.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GymProje.Data.Context
{
    // IdentityDbContext: Kullanıcı giriş işlemleri için gerekli tabloları otomatik getirir.
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Tablolarımızı tanımlıyoruz
        public DbSet<Gym> Gyms { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<GymService> GymServices { get; set; }
        public DbSet<Trainer> Trainers { get; set; }
        public DbSet<TrainerAvailability> TrainerAvailabilities { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<AIWorkoutPlan> AIWorkoutPlans { get; set; }

        // Veritabanı oluşurken yapılacak ince ayarlar
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder); // Identity ayarlarının bozulmaması için şart!

            // 1. Para birimi (Decimal) için SQL ayarı
            builder.Entity<GymService>()
                .Property(x => x.Price)
                .HasColumnType("decimal(18,2)");

            // 2. İlişki Ayarları (Silme Koruması)
            // Hoca silinirse randevuları silinmesin, hata versin.
            builder.Entity<Appointment>()
                .HasOne(a => a.Trainer)
                .WithMany(t => t.Appointments)
                .HasForeignKey(a => a.TrainerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Üye silinirse randevuları silinmesin.
            builder.Entity<Appointment>()
                .HasOne(a => a.AppUser)
                .WithMany()
                .HasForeignKey(a => a.AppUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Hizmet silinirse randevular etkilenmesin.
            builder.Entity<Appointment>()
                .HasOne(a => a.GymService)
                .WithMany()
                .HasForeignKey(a => a.GymServiceId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}