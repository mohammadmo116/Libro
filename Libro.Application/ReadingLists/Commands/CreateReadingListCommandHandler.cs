using Libro.Application.Interfaces;
using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using Libro.Infrastructure;
using Libro.Infrastructure.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Application.ReadingLists.Commands
{
    public sealed class CreateReadingListCommandHandler : IRequestHandler<CreateReadingListCommand, ReadingList>
    {
        private readonly IReadingListRepository _readingListRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<CreateReadingListCommandHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public CreateReadingListCommandHandler(
             IReadingListRepository readingListRepository,
             IUserRepository userRepository,
             ILogger<CreateReadingListCommandHandler> logger,
             IUnitOfWork unitOfWork
            ) {
            _readingListRepository = readingListRepository;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
        }
        public async Task<ReadingList> Handle(CreateReadingListCommand request, CancellationToken cancellationToken)
        {
           var user =await _userRepository.GetUserAsync(request.UserId);
            if (user is null) {
                _logger.LogInformation("CustomNotFoundException (User)");
                throw new CustomNotFoundException("User");
            }

            request.ReadingList.Id = Guid.NewGuid();
            request.ReadingList.UserId = request.UserId;
            await _readingListRepository.CreateReadingListAsync(request.ReadingList);
            await _unitOfWork.SaveChangesAsync();
            return request.ReadingList;
        }
    }
}
