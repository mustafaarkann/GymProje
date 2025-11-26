using System;
using System.ComponentModel.DataAnnotations;

namespace GymProje.Entity.Entities
{
    // Randevunun durumunu takip etmek için Enum (Sabit Liste)
    public enum AppointmentStatus
    {
        Pending,    // Onay Bekliyor
        Confirmed,  // Onaylandı
        Cancelled,  // İptal Edildi
        Completed   // Tamamlandı (Hizmet verildi)
    }

    public class Appointment : BaseEntity
    {
        // Randevuyu alan Üye
        // IdentityUser string ID kullandığı için int değil string tanımladık.
        public string AppUserId { get; set; } = null!;
        public AppUser AppUser { get; set; } = null!;

        // Hizmeti veren Hoca
        public int TrainerId { get; set; }
        public Trainer Trainer { get; set; } = null!;

        // Seçilen Hizmet (Fiyatı ve süresi buradan belli olacak)
        public int GymServiceId { get; set; }
        public GymService GymService { get; set; } = null!;

        public DateTime AppointmentDate { get; set; } // Tarih (Örn: 25.11.2025)
        public TimeSpan StartTime { get; set; } // Başlangıç Saati (14:00)
        public TimeSpan EndTime { get; set; } // Bitiş Saati (15:00) - Çakışma kontrolü için kritik

        public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending; // Varsayılan: Bekliyor
    }
}