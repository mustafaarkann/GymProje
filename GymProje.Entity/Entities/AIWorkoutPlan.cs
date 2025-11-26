using System.ComponentModel.DataAnnotations;

namespace GymProje.Entity.Entities
{
    public class AIWorkoutPlan : BaseEntity
    {
        // Rapor kime ait?
        public string AppUserId { get; set; } = null!;
        public AppUser AppUser { get; set; } = null!;

        // Kullanıcının girdiği bilgiler (Snapshot)
        // Örn: "Boy: 180, Kilo: 85, Hedef: Kilo Verme"
        [Required]
        public string UserPrompt { get; set; } = null!;

        // Yapay Zekadan (OpenAI) dönen uzun cevap
        [Required]
        public string AIResponse { get; set; } = null!;
    }
}