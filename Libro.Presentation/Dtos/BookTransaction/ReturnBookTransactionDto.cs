﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Presentation.Dtos.BookTransaction
{
    public class ReturnBookTransactionDto : ReserveBookTransactionDto
    {
    
        public Guid Id { get; set; }
    }
}