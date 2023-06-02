using Libro.Application.Interfaces;
using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Libro.Application.Users.Queries
{
    public class GetPatronUserQueryHandler : IRequestHandler<GetPatronUserQuery, User>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<GetPatronUserQueryHandler> _logger;

        public GetPatronUserQueryHandler(
            ILogger<GetPatronUserQueryHandler> logger,
            IUserRepository userRepository     
            ) 
        {
            _logger = logger;
            _userRepository = userRepository;
          
        }
        public async Task<User> Handle(GetPatronUserQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserWtithRolesAsync(request.UserId);
            if (user is null || ! user.Roles.Any(r=>r.Name.ToLower()=="patron")) {
                _logger.LogInformation("CustomNotFoundException (User)");
                throw new CustomNotFoundException("User");
              
            }
            return user;
            throw new NotImplementedException();
        }
    }
}
