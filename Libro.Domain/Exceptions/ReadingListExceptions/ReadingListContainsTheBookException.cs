using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Domain.Exceptions.ReadingListExceptions
{
    public class ReadingListContainsTheBookException : Exception
    {
        public ReadingListContainsTheBookException() : base("ReadingList Already Has The Specific Book") {
        
        } 
        
    }
}
