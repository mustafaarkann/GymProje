using System;

namespace GymProje.Entity.Entities
{
    public class TrainerAvailability : BaseEntity
    {
        public int TrainerId { get; set; }
        public Trainer Trainer { get; set; } = null!;

        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool IsFullDay { get; set; }
    }
}