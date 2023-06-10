using Libro.Application.Interfaces;
using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Libro.Application.Users.Queries
{
    public sealed class GetPatronBorrowingHistoryQueryHandler : IRequestHandler<GetPatronBorrowingHistoryQuery, (List<BookTransaction>, int)>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<GetPatronBorrowingHistoryQueryHandler> _logger;

        public GetPatronBorrowingHistoryQueryHandler(
            ILogger<GetPatronBorrowingHistoryQueryHandler> logger,
            IUserRepository userRepository
            )
        {
            _logger = logger;
            _userRepository = userRepository;

        }

        public async Task<(List<BookTransaction>, int)> Handle(GetPatronBorrowingHistoryQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserWtithRolesAsync(request.PatronId);
            if (user is null || !user.Roles.Any(r => r.Name.ToLower() == "patron"))
            {

                _logger.LogInformation("CustomNotFoundException (User)");
                throw new CustomNotFoundException("User");
            }

            return await _userRepository.GetBorrowingHistoryAsync(request.PatronId, request.PageNumber, request.Count);
        }
    }
}
