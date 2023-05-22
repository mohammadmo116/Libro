using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Libro.Presentation.Dtos.User
{
    public class CreateUserDto : UserDto
    {

        [PasswordPropertyText]
        [Required]
        public string Password { get; set; }

    }
}
