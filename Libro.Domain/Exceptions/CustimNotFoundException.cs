using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Domain.Exceptions
{
    public class CustomNotFoundException : Exception
    {
        public CustomNotFoundException(string code)
            : base($"404 {code} NotFound")
        {
        }
    }
}
