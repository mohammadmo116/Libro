using System.ComponentModel.DataAnnotations;

namespace Libro.Presentation.Dtos.Author
{
    public class CreateAuthorDto
    {
        [Required]
        [MaxLength(256)]
        public string? Name { get; set; }
        public DateTime? DateOfBirth { get; set; }

    }
}
