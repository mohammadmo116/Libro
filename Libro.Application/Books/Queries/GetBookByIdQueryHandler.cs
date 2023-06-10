using Libro.Application.Interfaces;
using Libro.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Libro.Application.Books.Queries
{
    public class GetBookByIdQueryHandler : IRequestHandler<GetBookByIdQuery, Book>
    {
        private readonly ILogger<GetBookByIdQueryHandler> _logger;
        private readonly IBookRepository _bookRepository;
        public GetBookByIdQueryHandler(ILogger<GetBookByIdQueryHandler> logger,
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
