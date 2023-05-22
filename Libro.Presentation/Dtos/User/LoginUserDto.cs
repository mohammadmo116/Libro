using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Libro.Presentation.Dtos.User
{
    public class LoginUserDto
    {
        [Required]
        [MaxLength(256)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [PasswordPropertyText]
        [Required]
        public string Password { get; set; }
    }
}
