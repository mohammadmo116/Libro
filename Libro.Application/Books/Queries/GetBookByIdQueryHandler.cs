using Libro.Application.Interfaces;
using Libro.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Application.Books.Queries
{
    public class GetBookByIdQueryHandler : IRequestHandler<GetBookByIdQuery,Book>
    {
        private readonly ILogger _logger;
        private readonly IBookRepository _bookRepository;
        public GetBookByIdQueryHandler(ILogger<Book> logger,
            IBookRepository bookRepository) 
        {
            _logger = logger;
            _bookRepository = bookRepository;
        }

        public async Task<Book> Handle(GetBookByIdQuery request, CancellationToken cancellationToken)
        {
            return await _bookRepository.GetBookAsync(request.BookId);
 
        }
    }
}
