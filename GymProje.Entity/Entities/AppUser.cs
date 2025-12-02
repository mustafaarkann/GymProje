using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;

namespace GymProje.Entity.Entities
{
    // IdentityUser sınıfından miras alıyoruz.
    // Bu sayede Email, PasswordHash, PhoneNumber gibi alanlar otomatik geliyor.
    public class AppUser : IdentityUser
    {
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = null!; // Zorunlu

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = null!; // Zorunlu

        public DateTime? BirthDate { get; set; } // Yaş hesaplaması için gerekli (AI önerisi için)
        //commit
        //commit
        public string? Gender { get; set; } // Cinsiyet (AI önerisi için)
    }
}