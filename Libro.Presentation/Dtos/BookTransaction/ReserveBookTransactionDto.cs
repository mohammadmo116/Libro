using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Libro.Presentation.Dtos.BookTransaction
{
    public class ReserveBookTransactionDto
    {
        [Required]
        [ForeignKey(nameof(Book))]
        public Guid BookId { get; set; }
        [Required]
        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }
    }
}
