using Libro.Application.Repositories;
using Libro.Domain.Exceptions;
using Libro.Infrastructure;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Libro.Application.Authors.Commands
{
    public class RemoveAuthorCommandHandler : IRequestHandler<RemoveAuthorCommand, bool>
    {
        private readonly ILogger<RemoveAuthorCommandHandler> _logger;
        private readonly IAuthorRepository _authorRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RemoveAuthorCommandHandler(
            ILogger<RemoveAuthorCommandHandler> logger,
            IAuthorRepository authorRepository,
            IUnitOfWork unitOfWork
            )
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _authorRepository = authorRepository;
        }


        public async Task<bool> Handle(RemoveAuthorCommand request, CancellationToken cancellationToken)
        {
            var author = await _authorRepository.GetAuthorAsync(request.AuthorId);
            if (author is null)
            {
                _logger.LogInformation($"CustomNotFoundException BookId:{request.AuthorId}");
                throw new CustomNotFoundException("Author");
            }

            _authorRepository.RemoveAuthor(author);
            var numberOfRows = await _unitOfWork.SaveChangesAsync();
            return numberOfRows > 0;
        }
    }
}
