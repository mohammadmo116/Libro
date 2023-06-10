using System.ComponentModel.DataAnnotations;

namespace Libro.Presentation.Dtos.ReadingList
{
    public class ReadingListDto
    {
        public string Name { get; set; }
        [Required]
        public bool Private { get; set; }
    }
}
