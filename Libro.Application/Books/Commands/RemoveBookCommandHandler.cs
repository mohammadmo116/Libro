using Libro.Application.Interfaces;
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
    public sealed class RemoveBookCommandHandler : IRequestHandler<RemoveBookCommand, bool>
    {

        private readonly ILogger<RemoveBookCommandHandler> _logger;
        private readonly IBookRepository _bookRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RemoveBookCommandHandler(
            ILogger<RemoveBookCommandHandler> logger,
            IBookRepository bookRepository,
            IUnitOfWork unitOfWork
            )
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _bookRepository = bookRepository;
        }
        public Task<bool> Handle(RemoveBookCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
