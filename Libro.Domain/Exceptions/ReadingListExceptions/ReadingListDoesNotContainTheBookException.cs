using Libro.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Domain.Exceptions.ReadingListExceptions
{
    public class ReadingListDoesNotContainTheBookException : Exception
    {
        public ReadingListDoesNotContainTheBookException() :base("ReadingList Does Not Contain The Book") 
        {
        }
    }
}
