using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Presentation.Dtos.ReadingList
{
    public class UpdateReadingListDto 
    {
        public Guid Id { get; set; }
        [Required]
        [MaxLength(250)]
        public string Name { get; set; }
        public bool? Private { get; set; }
    }
}
