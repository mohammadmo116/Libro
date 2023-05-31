using Libro.Application.Interfaces;
using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using Libro.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Libro.Application.Users.Commands
{

    public sealed class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, User>
    {
        private readonly IAuthenticationRepository _authenticationRepository;
        private readonly ILogger<CreateUserCommandHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;
        public CreateUserCommandHandler(ILogger<CreateUserCommandHandler> logger,
            IAuthenticationRepository authenticationRepository,
            IUnitOfWork unitOfWork)
        {
            _authenticationRepository = authenticationRepository;
            _logger= logger;
            _unitOfWork= unitOfWork;
        }
      
        public async Task<User> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                 await _authenticationRepository.ExceptionIfUserExistsAsync(request.User);   
                 var user=await _authenticationRepository.RegisterUserAsync(request.User);
                 await _unitOfWork.SaveChangesAsync();
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
