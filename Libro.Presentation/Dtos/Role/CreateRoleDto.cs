using System.ComponentModel.DataAnnotations;

namespace Libro.Presentation.Dtos.Role
{
    public class CreateRoleDto 
    {
        [Required]
        [MaxLength(256)]
        public string Name { get; set; }
      
    }
}
