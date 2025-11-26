using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GymProje.Entity.Entities
{
    public class Service : BaseEntity
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = null!;

        public string? Description { get; set; } // Açıklama girmek zorunlu olmasın diye '?' koyduk.

        public ICollection<GymService> GymServices { get; set; } = new List<GymService>();
        public ICollection<TrainerService> TrainerServices { get; set; } = new List<TrainerService>();
    }
}