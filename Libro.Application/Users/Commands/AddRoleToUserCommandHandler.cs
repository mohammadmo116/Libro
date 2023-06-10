using Libro.Application.Interfaces;
using Libro.Domain.Exceptions.UserExceptions;
using Libro.Infrastructure;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Libro.Application.Users.Commands
{
    public sealed class AddRoleToUserCommandHandler : IRequestHandler<AddRoleToUserCommand, bool>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<AddRoleToUserCommandHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;
        public AddRoleToUserCommandHandler(ILogger<AddRoleToUserCommandHandler> logger,
                                           IUserRepository userRepository,
                                           IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(AddRoleToUserCommand request, CancellationToken cancellationToken)
        {

            if (await _userRepository.RoleOrUserNotFoundAsync(request.UserRole))
            {
                _logger.LogInformation($"UserOrRoleNotFoundException");
                _logger.LogInformation($"UserId : {request.UserRole.UserId}, RoleId : {request.UserRole.RoleId}");

                throw new UserOrRoleNotFoundException();
            }
            if (await _userRepository.UserHasTheAssignedRoleAsync(request.UserRole))
            {
                _logger.LogInformation($"UserHasTheAssignedRoleException message");
                _logger.LogInformation($"UserId : {request.UserRole.UserId}, RoleId : {request.UserRole.RoleId}");

                throw new UserHasTheAssignedRoleException();
            }
            await _userRepository.AssignRoleToUserAsync(request.UserRole);
            var numberOfRows = await _unitOfWork.SaveChangesAsync();
            return numberOfRows > 0;






        }
    }
}
