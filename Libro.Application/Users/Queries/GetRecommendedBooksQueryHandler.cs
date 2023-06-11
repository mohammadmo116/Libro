using Libro.Application.Interfaces;
using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Libro.Application.Users.Queries
{
    public sealed class GetRecommendedBooksQueryHandler : IRequestHandler<GetRecommendedBooksQuery, (List<Book>, int)>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<GetRecommendedBooksQueryHandler> _logger;

        public GetRecommendedBooksQueryHandler(
            ILogger<GetRecommendedBooksQueryHandler> logger,
            IUserRepository userRepository
            )
        {
            _logger = logger;
            _userRepository = userRepository;

        }

        public async Task<(List<Book>, int)> Handle(GetRecommendedBooksQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserAsync(request.UserId);
            if (user is null)
            {
                _logger.LogInformation("CustomNotFoundException (User)");
                throw new CustomNotFoundException("User");
            }
            return await _userRepository.GetRecommendedBooksAsync(user.Id, request.PageNumber, request.Count);
        }
    }
}
