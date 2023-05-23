using Libro.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
