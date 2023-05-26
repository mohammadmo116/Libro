using Libro.Domain.Common;
using Libro.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Domain.Entities
{
    public class BookTransaction : BaseEntity
    {
        [Required]
        [ForeignKey(nameof(Book))]
        public Guid BookId { get; set; }
        [Required]
        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }
        [Required]
        public BookStatus Status { get; set; }
        public DateTime? BorrowedDate { get; set; }
        public DateTime? DueDate { get; set; }

    }
}
