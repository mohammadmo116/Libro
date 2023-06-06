﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Domain.Exceptions.BookExceptions
{
    public class BookIsNotReservedException : Exception
    {
        public BookIsNotReservedException() : base("Please Reserve The Book So you can borrow it")
        {
        }
    }
}