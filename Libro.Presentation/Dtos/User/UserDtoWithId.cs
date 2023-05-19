using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Presentation.Dtos.User
{
    public class UserDtoWithId: UserDto
    {
        public Guid Id { get; set; }
    }
}
