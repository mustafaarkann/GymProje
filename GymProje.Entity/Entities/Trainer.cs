using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GymProje.Entity.Entities
{
    public class Trainer : BaseEntity
    {
        [Required(ErrorMessage = "Antrenör adı soyadı zorunludur.")]
        [StringLength(50)]
        public string FullName { get; set; } = null!;

        public string? Specialty { get; set; } // Uzmanlık girmek şart değil

        public string? ImageUrl { get; set; } // Resim olmayabilir

        public int GymId { get; set; }
        public Gym Gym { get; set; } = null!; // Hangi salona bağlı olduğu zorunlu

        public ICollection<TrainerService> TrainerServices { get; set; } = new List<TrainerService>();
        public ICollection<TrainerAvailability> Availabilities { get; set; } = new List<TrainerAvailability>();

        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}