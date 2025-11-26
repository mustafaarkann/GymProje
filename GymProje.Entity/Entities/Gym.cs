using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GymProje.Entity.Entities
{
    public class Gym : BaseEntity
    {
        [Required(ErrorMessage = "Salon adı zorunludur.")]
        [StringLength(100)]
        public string Name { get; set; } = null!; // Veritabanı dolduracak.

        [Required]
        public string Address { get; set; } = null!;

        public TimeSpan OpeningTime { get; set; }
        public TimeSpan ClosingTime { get; set; }

        // Listeleri mutlaka başlatmalıyız!
        public ICollection<GymService> GymServices { get; set; } = new List<GymService>();
        public ICollection<Trainer> Trainers { get; set; } = new List<Trainer>();
    }
}