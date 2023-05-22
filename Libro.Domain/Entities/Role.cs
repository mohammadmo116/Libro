using Libro.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace Libro.Domain.Entities
{
    public class Role : BaseEntity
    {
       
        [Required]
        [MaxLength(256)]
        public string Name { get; set; }
        public List<User>? Users { get; set; } = new();
    }
}
