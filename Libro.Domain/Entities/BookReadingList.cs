using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Domain.Entities
{
    public class BookReadingList
    {
        [Required]
        public Guid BookId { get; set; }

        [Required]
        public Guid ReadingListId { get; set; }

        public Book Book { get; set; } = null!;
        public ReadingList ReadingList { get; set; } = null!;

    }
}
