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
        public string? Title { get; set; }

        public DateTime? PublishedDate { get; set; }

        public List<Author>? Authors { get; set; }
        public bool IsAvailable { get; set; } = true;

    }
}
