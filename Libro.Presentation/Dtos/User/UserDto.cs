using System.ComponentModel.DataAnnotations;

namespace Libro.Presentation.Dtos.User
{
    public class UserDto
    {
       
        [MaxLength(256)]
        [Required]
        public string? UserName { get; set; }
        [Required]
        [MaxLength(256)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Phone]
        public string? PhoneNumber { get; set; } 


    }
}
