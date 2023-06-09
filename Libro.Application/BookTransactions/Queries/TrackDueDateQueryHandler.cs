﻿using Libro.Application.Interfaces;
using Libro.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Libro.Application.BookTransactions.Queiries
{
    public sealed class TrackDueDateQueryHandler : IRequestHandler<TrackDueDateQuery, (List<BookTransaction>, int)>
    {
        private readonly ILogger<TrackDueDateQueryHandler> _logger;
        private readonly IBookTransactionRepository _bookTransactionRepository;

        public TrackDueDateQueryHandler(ILogger<TrackDueDateQueryHandler> logger,
            IBookTransactionRepository bookTransactionRepository)
        {
            _logger = logger;
            _bookTransactionRepository = bookTransactionRepository;
        }

        public async Task<(List<BookTransaction>, int)> Handle(TrackDueDateQuery request, CancellationToken cancellationToken)
        {
            return await _bookTransactionRepository.TrackDueDateAsync(request.PageNumber, request.Count);
        }
    }
}
