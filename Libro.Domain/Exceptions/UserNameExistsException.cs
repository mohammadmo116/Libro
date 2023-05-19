using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Domain.Exceptions
{
    public class UserNameExistsException : UserExistsException
    {
        public UserNameExistsException(string UserName)
            : base("UserName", UserName)
        {
        }
    }
}
