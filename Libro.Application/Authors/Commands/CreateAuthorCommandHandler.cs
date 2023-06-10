using Libro.Application.Repositories;
using Libro.Domain.Entities;
using Libro.Infrastructure;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Libro.Application.Authors.Commands
{
    public sealed class CreateAuthorCommandHandler : IRequestHandler<CreateAuthorCommand, Author>
    {
        private readonly ILogger<CreateAuthorCommandHandler> _logger;
        private readonly IAuthorRepository _authorRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateAuthorCommandHandler(
            ILogger<CreateAuthorCommandHandler> logger,
            IAuthorRepository authorRepository,
            IUnitOfWork unitOfWork
            )
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _authorRepository = authorRepository;
        }

        public async Task<Author> Handle(CreateAuthorCommand request, CancellationToken cancellationToken)
        {
            request.author.Id = Guid.NewGuid();
            await _authorRepository.CreateAuthorAsync(request.author);
            await _unitOfWork.SaveChangesAsync();
            return request.author;
        }
    }
}
