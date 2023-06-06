using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Presentation.Dtos.ReadingList
{
    public class ReadingListDto
    {
        public string Name { get; set; }
        [Required]
        public bool Private { get; set; }
    }
}
