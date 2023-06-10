using Libro.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace Libro.Domain.Entities
{
    public class Notification : BaseEntity
    {
        [Required]
        public string Message { get; set; }
        public bool IsRead { get; set; } = false;
        public Guid? UserId { get; set; }
        public User? User { get; set; }
    }
}
