using Libro.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Domain.Entities
{
    public class Book : BaseEntity
    {
        [Required]
        [MaxLength(256)]
        public string Title { get; set; }

        public string? Genre { get; set; }
        public DateTime? PublishedDate { get; set; }

        public List<Author>? Authors { get; set; }
        public bool? IsAvailable { get; set; }
        public List<User>? Users { get; set; }= new();
        public List<ReadingList>? ReadingLists { get; set; }
        public List<BookTransaction>? BookTransactions { get; set; } = new();
        public List<BookReview>? Reviews { get; set; }
    }
}
