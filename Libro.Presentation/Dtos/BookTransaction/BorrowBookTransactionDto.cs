using Libro.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Presentation.Dtos.BookTransaction
{
    public class BorrowBookTransactionDto : ReserveBookTransactionDto
    {
        
        public Guid Id { get; set; }
        public DateTime DueDate { get; set; }

    }
}
