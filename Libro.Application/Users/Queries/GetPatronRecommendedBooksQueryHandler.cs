using Libro.Application.Interfaces;
using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Libro.Application.Users.Queries
{
    public sealed class GetPatronRecommendedBooksQueryHandler : IRequestHandler<GetPatronRecommendedBooksQuery, (List<Book>, int)>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<GetPatronRecommendedBooksQueryHandler> _logger;

        public GetPatronRecommendedBooksQueryHandler(
            ILogger<GetPatronRecommendedBooksQueryHandler> logger,
            IUserRepository userRepository
            )
        {
            _logger = logger;
            _userRepository = userRepository;

        }

        public async Task<(List<Book>, int)> Handle(GetPatronRecommendedBooksQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserWtithRolesAsync(request.PatronId);
            if (user is null || !user.Roles.Any(r => r.Name.ToLower() == "patron"))
            {
                _logger.LogInformation("CustomNotFoundException (User)");
                throw new CustomNotFoundException("User");
            }
            return await _userRepository.GetRecommendedBooksAsync(user.Id, request.PageNumber, request.Count);
        }
    }
}
