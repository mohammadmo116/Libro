using Libro.Application.Interfaces;
using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Libro.Application.Users.Commands
{
    public sealed class AddRoleToUserCommandHandler : IRequestHandler<AddRoleToUserCommand,List<string>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<AddRoleToUserCommandHandler> _logger;

        public AddRoleToUserCommandHandler(ILogger<AddRoleToUserCommandHandler> logger,
                                           IUserRepository userRepository)
        {
            _logger = logger;
            _userRepository = userRepository;
        }

        public async Task<List<string>> Handle(AddRoleToUserCommand request, CancellationToken cancellationToken)
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
                var Roles = await _userRepository.AssignRoleToUserAsync(request.UserRole);
                return Roles;
            
        



        }
    }
}
