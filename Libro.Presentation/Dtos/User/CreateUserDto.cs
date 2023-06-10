using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Libro.Presentation.Dtos.User
{
    public class CreateUserDto : UserDto
    {

        [PasswordPropertyText]
        [Required]
        public string Password { get; set; }

    }
}
