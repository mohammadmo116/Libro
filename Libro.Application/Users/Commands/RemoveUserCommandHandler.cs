using Libro.Application.Interfaces;
using Libro.Domain.Exceptions;
using Libro.Infrastructure;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Libro.Application.Users.Commands
{
    public class RemoveUserCommandHandler : IRequestHandler<RemoveUserCommand, bool>
    {
        private readonly ILogger<RemoveUserCommandHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepository;

        public RemoveUserCommandHandler(
            ILogger<RemoveUserCommandHandler> logger,
              IUserRepository userRepository,
              IUnitOfWork unitOfWork
            )
        {
            _logger = logger;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> Handle(RemoveUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserAsync(request.UserId);

            if (user is null)
            {
                _logger.LogInformation($"CustomNotFoundException UserId:{request.UserId}");
                throw new CustomNotFoundException($"User");

            }

            _userRepository.RemoveUser(user);
            var numberOfRows = await _unitOfWork.SaveChangesAsync();
            return numberOfRows > 1;
        }
    }
}
