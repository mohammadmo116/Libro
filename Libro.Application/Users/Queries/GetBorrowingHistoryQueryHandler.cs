using Libro.Application.Interfaces;
using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Application.Users.Queries
{
    public sealed class GetBorrowingHistoryQueryHandler : IRequestHandler<GetBorrowingHistoryQuery, (List<BookTransaction>, int)>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<GetBorrowingHistoryQueryHandler> _logger;

        public GetBorrowingHistoryQueryHandler(
            ILogger<GetBorrowingHistoryQueryHandler> logger,
            IUserRepository userRepository
            )
        {
            _logger = logger;
            _userRepository = userRepository;

        }
        public async Task<(List<BookTransaction>,int)> Handle(GetBorrowingHistoryQuery request, CancellationToken cancellationToken)
        {
            var user=await _userRepository.GetUserAsync(request.UserId);
            if (user is null) {
                _logger.LogInformation("CustomNotFoundException (User)");
                throw new CustomNotFoundException("User");
            }
            return await _userRepository.GetBorrowingHistoryAsync(request.UserId, request.PageNumber, request.Count);

        }
  
    }
}
