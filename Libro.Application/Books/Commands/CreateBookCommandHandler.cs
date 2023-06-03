using Libro.Application.Interfaces;
using Libro.Domain.Entities;
using Libro.Infrastructure;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Application.Books.Commands
{
    public sealed class CreateBookCommandHandler : IRequestHandler<CreateBookCommand, Book>
    {
        private readonly ILogger<CreateBookCommandHandler> _logger;
        private readonly IBookRepository _bookRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateBookCommandHandler(
            ILogger<CreateBookCommandHandler> logger,
            IBookRepository bookRepository,
            IUnitOfWork unitOfWork
            ) 
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _bookRepository = bookRepository;
        }

        public async Task<Book> Handle(CreateBookCommand request, CancellationToken cancellationToken)
        {
          
            request.book.Id = Guid.NewGuid();
            await _bookRepository.CreateBookAsync(request.book);
            await _unitOfWork.SaveChangesAsync();
            return request.book;
        }
    }
}
