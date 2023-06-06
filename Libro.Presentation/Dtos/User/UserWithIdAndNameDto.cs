using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Presentation.Dtos.User
{
    public class UserWithIdAndNameDto
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }

    }
}
