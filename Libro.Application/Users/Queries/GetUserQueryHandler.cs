using Libro.Application.Interfaces;
using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Libro.Application.Users.Queries
{
    public class GetUserQueryHandler : IRequestHandler<GetUserQuery, User>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<GetUserQueryHandler> _logger;

        public GetUserQueryHandler(
            ILogger<GetUserQueryHandler> logger,
            IUserRepository userRepository     
            ) 
        {
            _logger = logger;
            _userRepository = userRepository;
          
        }
        public async Task<User> Handle(GetUserQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserAsync(request.UserId);
            if (user is null || ! user.Roles.Any(r=>r.Name.ToLower()=="patron")) {
                throw new CustomNotFoundException("User");
            }
            return user;
            throw new NotImplementedException();
        }
    }
}
