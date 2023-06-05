using Libro.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Domain.Entities
{
    public class ReadingList : BaseEntity
    {
        [Required]
        [MaxLength(250)]
        public string Name { get; set; }

        public List<Book>? Books { get; set; }
        public User? User { get; set; }

    }
}
