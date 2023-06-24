using Libro.Application.Interfaces;
using Libro.Domain.Entities;
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

            return await _userRepository.GetRecommendedBooksAsync(request.UserId, request.PageNumber, request.Count);
        }
    }
}
