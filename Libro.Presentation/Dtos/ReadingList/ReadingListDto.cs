using System.ComponentModel.DataAnnotations;

namespace Libro.Presentation.Dtos.ReadingList
{
    public class ReadingListDto
    {
        [Required]
        public string Name { get; set; }
       
        public bool Private { get; set; } = true;
    }
}
