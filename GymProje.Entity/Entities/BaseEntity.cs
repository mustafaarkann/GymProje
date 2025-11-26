using System;

namespace GymProje.Entity.Entities
{
    // abstract yaptık ki bu sınıftan direkt nesne üretilmesin  sadece miras alınsın.
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now; // Oluştuğu an tarihi atar.
    }
}