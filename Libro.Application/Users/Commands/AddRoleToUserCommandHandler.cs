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
        private readonly ILogger<User> _logger;

        public AddRoleToUserCommandHandler(ILogger<User> logger,
                                           IUserRepository userRepository)
        {
            _logger = logger;
            _userRepository = userRepository;
        }

        public async Task<List<string>> Handle(AddRoleToUserCommand request, CancellationToken cancellationToken)
        {
            try
            {

                var Roles = await _userRepository.AssignRoleToUserAsync(request.UserRole);
                return Roles;
            }
            catch (UserOrRoleNotFoundException e)
            {
                _logger.LogInformation($"UserOrRoleNotFoundException {e.Message}");
                _logger.LogInformation($"UserId : {request.UserRole.UserId}, RoleId : {request.UserRole.RoleId}");

                throw e;
            }
            catch (UserHasTheAssignedRoleException e)
            {
                
                _logger.LogInformation($"UserHasTheAssignedRoleException message:{e.Message}");
                _logger.LogInformation($"UserId : {request.UserRole.UserId}, RoleId : {request.UserRole.RoleId}");
                throw e;
            }



        }
    }
}
