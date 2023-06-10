using Libro.Domain.Common;
using Libro.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Libro.Domain.Entities
{
    public class BookTransaction : BaseEntity
    {

        [Required]
        public Guid BookId { get; set; }

        [Required]
        public Guid UserId { get; set; }

        public Book Book { get; set; } = null!;
        public User User { get; set; } = null!;


        [Required]
        public BookStatus Status { get; set; }
        public DateTime? BorrowedDate { get; set; }
        public DateTime? DueDate { get; set; }

    }
}
