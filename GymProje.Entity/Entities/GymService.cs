using System.ComponentModel.DataAnnotations.Schema;

namespace GymProje.Entity.Entities
{
    public class GymService : BaseEntity
    {
        public int GymId { get; set; }
        public Gym Gym { get; set; } = null!; // Bu ilişki zorunlu

        public int ServiceId { get; set; }
        public Service Service { get; set; } = null!; // Bu ilişki zorunlu

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public int DurationMinutes { get; set; }
    }
}