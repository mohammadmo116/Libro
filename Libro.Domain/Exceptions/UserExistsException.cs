using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Domain.Exceptions
{
    public class UserExistsException: Exception
    {
        public string _message;
        public string _field;
        public UserExistsException(string field,string message)
          : base($"{field} is used \n {field}: {message}")
        {
            _field = field;
            _message = message;
        }
    }
}
