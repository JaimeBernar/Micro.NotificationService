namespace NotificationService.Models
{
    using System.ComponentModel.DataAnnotations;

    public abstract class Entity
    {
        [Required]
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
    }
}
