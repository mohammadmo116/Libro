using Libro.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace Libro.Domain.Entities
{
    public class ReadingList : BaseEntity
    {

        public ReadingList()
        {
            Books = new List<Book>();
        }
        [Required]
        [MaxLength(250)]
        public string Name { get; set; }
        public bool? Private { get; set; }
        public List<Book> Books { get; set; }
        [Required]
        public Guid UserId { get; set; }
        public User User { get; set; }

    }
}
