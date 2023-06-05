using Libro.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Reflection.Metadata.BlobBuilder;

namespace Libro.Domain.Entities
{
    public class ReadingList : BaseEntity
    { 

        public ReadingList() {
        Books = new List<Book>();
      }
        [Required]
        [MaxLength(250)]
        public string Name { get; set; }

        public List<Book> Books { get; set; }
        [Required]
        public Guid UserId { get; set; }
        public User User { get; set; }
      
    }
}
