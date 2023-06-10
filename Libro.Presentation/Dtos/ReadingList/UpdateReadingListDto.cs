using System.ComponentModel.DataAnnotations;

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
