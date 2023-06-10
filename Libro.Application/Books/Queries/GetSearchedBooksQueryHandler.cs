using Libro.Application.Interfaces;
using Libro.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;


namespace Libro.Application.Books.Queries
{
    public sealed class GetSearchedBooksQueryHandler : IRequestHandler<GetSearchedBooksQuery, (List<Book>, int)>
    {
        private readonly ILogger<GetSearchedBooksQueryHandler> _logger;
        private readonly IBookRepository _bookRepository;
        public GetSearchedBooksQueryHandler(ILogger<GetSearchedBooksQueryHandler> logger,
            IBookRepository bookRepository)
        {
            _logger = logger;
            _bookRepository = bookRepository;

        }


        public async Task<(List<Book>, int)> Handle(GetSearchedBooksQuery request, CancellationToken cancellationToken)
        {
            if (request.Title is null && request.AuthorName is null && request.Genre is null)
                return await _bookRepository.GetAllBooksAsync(request.PageNumber, request.Count);


            var a = await _bookRepository.GetSearchedBooksAsync(request.Title, request.AuthorName, request.Genre, request.PageNumber, request.Count);
            return a;

        }
    }
}
