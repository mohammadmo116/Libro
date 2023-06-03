using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Presentation.Dtos.User
{
    public class UpdateUserDto
    {
        public Guid Id { get; set; }

        [MaxLength(256)]
        public string? UserName { get; set; }

        [MaxLength(256)]
        [EmailAddress]
        public string? Email { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }
    }
}
