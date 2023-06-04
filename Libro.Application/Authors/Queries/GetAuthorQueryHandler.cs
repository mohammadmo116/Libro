using Libro.Application.Books.Queries;
using Libro.Application.Interfaces;
using Libro.Application.Repositories;
using Libro.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Application.Authors.Queries
{
    public class GetAuthorQueryHandler:IRequestHandler<GetAuthorQuery,Author>
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
