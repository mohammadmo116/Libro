using Libro.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Presentation.Dtos.User
{
    public class CreateUserDto : UserDto
    {
        [PasswordPropertyText]
        [Required]
        public string PasswordHash { get; set; }

    }
}
