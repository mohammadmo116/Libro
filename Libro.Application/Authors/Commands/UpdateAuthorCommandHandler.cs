using Libro.Application.Interfaces;
using Libro.Application.Repositories;
using Libro.Domain.Exceptions;
using Libro.Infrastructure;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Application.Authors.Commands
{
    public class UpdateAuthorCommandHandler : IRequestHandler<UpdateAuthorCommand, bool>
    {
        private readonly ILogger<UpdateAuthorCommandHandler> _logger;
        private readonly IAuthorRepository _authorRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateAuthorCommandHandler(
            ILogger<UpdateAuthorCommandHandler> logger,
            IAuthorRepository authorRepository,
            IUnitOfWork unitOfWork
            )
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _authorRepository = authorRepository;
        }
        public async Task<bool> Handle(UpdateAuthorCommand request, CancellationToken cancellationToken)
        {
            var author = await _authorRepository.GetAuthorAsync(request.author.Id);
            if (author is null)
            {
                _logger.LogInformation($"CustomNotFoundException BookId:{request.author.Id}");
                throw new CustomNotFoundException("Author");

            }
            author.Name = request.author.Name is null ?
              author.Name : request.author.Name;

            author.DateOfBirth = request.author.DateOfBirth is null ?
                author.DateOfBirth : request.author.DateOfBirth;


            _authorRepository.UpdateAuthor(author);
            var numberOfRows = await _unitOfWork.SaveChangesAsync();
            return numberOfRows > 0;
        }
    }
}
