using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Domain.Exceptions
{
    public class PhoneNumberExistsException : UserExistsException
    {
        public PhoneNumberExistsException(string PhoneNumber)
            : base("PhoneNumber", PhoneNumber)
        {
        }
    }
}
