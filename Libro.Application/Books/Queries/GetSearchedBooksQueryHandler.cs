using Libro.Application.Interfaces;
using Libro.Application.Roles.Commands;
using Libro.Application.Users.Queries;
using Libro.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;


namespace Libro.Application.Books.Queries
{
    public sealed class GetSearchedBooksQueryHandler : IRequestHandler<GetSearchedBooksQuery, List<string>>
    {
        private readonly ILogger _logger;
        private readonly IBookRepository _bookRepository;
        public GetSearchedBooksQueryHandler(ILogger<Book> logger,
            IBookRepository bookRepository)
        {
            _logger = logger;
            _bookRepository = bookRepository;

        }
  

        public async Task<List<string>> Handle(GetSearchedBooksQuery request, CancellationToken cancellationToken)
        {
            return await _bookRepository.GetBooks(Title:request.Title, AuthorName: request.AuthorName,Genre:request.Genre);
        }
    }
}
