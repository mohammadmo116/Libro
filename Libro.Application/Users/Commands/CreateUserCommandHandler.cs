using Libro.Application.Interfaces;
using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Libro.Application.Users.Commands
{

    public sealed class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, User>
    {
        private readonly IAuthenticationRepository _authenticationRepository;
        private readonly ILogger<CreateUserCommandHandler> _logger;

        public CreateUserCommandHandler(ILogger<CreateUserCommandHandler> logger,
            IAuthenticationRepository authenticationRepository)
        {
            _authenticationRepository = authenticationRepository;
            _logger= logger;
        }
      
        public async Task<User> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                 await _authenticationRepository.ExceptionIfUserExistsAsync(request.User);
                 var user = await _authenticationRepository.RegisterUserAsync(request.User);
                return user;
            }
            catch (UserExistsException e)
            {
                _logger.LogInformation($"UserExistsException message : {e.Message}");
                throw e; 
            }

        }
    }

}
