using Libro.Application.Repositories;
using Libro.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Libro.Application.Authors.Queries
{
    public class GetAuthorQueryHandler : IRequestHandler<GetAuthorQuery, Author>
    {
        private readonly ILogger<GetAuthorQueryHandler> _logger;
        private readonly IAuthorRepository _authorRepository;
        public GetAuthorQueryHandler(ILogger<GetAuthorQueryHandler> logger,
            IAuthorRepository authorRepository)
        {
            _logger = logger;
            _authorRepository = authorRepository;
        }

        public async Task<Author> Handle(GetAuthorQuery request, CancellationToken cancellationToken)
        {
            return await _authorRepository.GetAuthorAsync(request.AuthorId);
        }
    }
}
