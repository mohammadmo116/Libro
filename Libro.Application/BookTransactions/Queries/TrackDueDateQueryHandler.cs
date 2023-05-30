using Libro.Application.Interfaces;
using Libro.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Application.BookTransactions.Queiries
{
    public sealed class TrackDueDateQueryHandler : IRequestHandler<TrackDueDateQuery, List<BookTransaction>>
    {
        private readonly ILogger<BookTransaction> _logger;
        private readonly IBookTransactionRepository _bookTransactionRepository;

        public TrackDueDateQueryHandler(ILogger<BookTransaction> logger, 
            IBookTransactionRepository bookTransactionRepository)
        {
            _logger = logger;
            _bookTransactionRepository = bookTransactionRepository;
        }

        public async Task<List<BookTransaction>> Handle(TrackDueDateQuery request, CancellationToken cancellationToken)
        {
            return await _bookTransactionRepository.TrackDueDateAsync();
        }
    }
}
