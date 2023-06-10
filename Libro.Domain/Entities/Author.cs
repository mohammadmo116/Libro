using Libro.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace Libro.Domain.Entities
{
    public class Author : BaseEntity
    {
        [Required]
        [MaxLength(256)]
        public string? Name { get; set; }
        public DateTime? DateOfBirth { get; set; }

        public List<Book>? Books { get; set; }
    }
}
