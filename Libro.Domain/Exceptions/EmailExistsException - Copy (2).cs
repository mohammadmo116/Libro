using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Domain.Exceptions
{
    public class EmailExistsException : UserExistsException
    {
        public EmailExistsException(string email)
            : base("Email", email)
        {
        }
    }
}
