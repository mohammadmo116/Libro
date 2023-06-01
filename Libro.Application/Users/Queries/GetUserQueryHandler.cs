using Libro.Application.Interfaces;
using Libro.Domain.Entities;
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
        public Task<User> Handle(GetUserQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
