using Libro.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Domain.Entities
{
    public class BookReview 
    {
        public BookReview() {
            Book=new Book();
            User=new User();
        }

        [Range(1,5)]
        [Required]
        public short Rate { get; set; }
        public string? Review { get; set; }
        public Book Book { get; set; }
        public Guid BookId { get; set; }
        public User User { get; set; }
        public Guid UserId { get; set; }
    }
}
