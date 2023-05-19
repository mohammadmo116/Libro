using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Domain.Exceptions
{
    public class InvalidCredentialsException:Exception
    {
        public InvalidCredentialsException(string email,string Password)
            : base($"Invalid Credentials,\n email : {email} \n password: {Password}")
        {
        }
    }
}
