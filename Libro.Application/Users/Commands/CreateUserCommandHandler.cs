using Libro.Application.Interfaces;
using Libro.Domain.Entities;
using Libro.Domain.Exceptions.UserExceptions;
using Libro.Infrastructure;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Libro.Application.Users.Commands
{

    public sealed class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, User>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<CreateUserCommandHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;
        public CreateUserCommandHandler(ILogger<CreateUserCommandHandler> logger,
            IUserRepository userRepository,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<User> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {


            if (!await _userRepository.EmailIsUniqueAsync(request.User.Email))
            {
                _logger.LogInformation($"UserExistsException : Email is Used");
                throw new UserExistsException(nameof(request.User.Email));
            }
            if (!await _userRepository.UserNameIsUniqueAsync(request.User.UserName))
            {
                _logger.LogInformation($"UserExistsException : UserName is Used");
                throw new UserExistsException(nameof(request.User.UserName));
            }
            if (!await _userRepository.PhoneNumberIsUniqueAsync(request.User.PhoneNumber))
            {
                _logger.LogInformation($"UserExistsException : PhoneNumber is Used");
                throw new UserExistsException(nameof(request.User.PhoneNumber));
            }
            var user = await _userRepository.RegisterUserAsync(request.User);
            await _unitOfWork.SaveChangesAsync();
            return user;

        }
    }

}
