using Libro.Application.Interfaces;
using Libro.Domain.Exceptions;
using Libro.Infrastructure;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Application.Users.Commands
{
    public class RemoveUserByRoleCommandHandler : IRequestHandler<RemoveUserByRoleCommand, bool>
    {
        private readonly ILogger<RemoveUserByRoleCommandHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepository;

        public RemoveUserByRoleCommandHandler(
            ILogger<RemoveUserByRoleCommandHandler> logger,
              IUserRepository userRepository,
              IUnitOfWork unitOfWork
            )
        {
            _logger = logger;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<bool> Handle(RemoveUserByRoleCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserWtithRolesAsync(request.UserId);

            if (user is null || !user.Roles.Any(r => r.Name.ToLower() == request.RoleName.ToLower()))
            {
                _logger.LogInformation($"CustomNotFoundException UserId:{request.UserId}");
                throw new CustomNotFoundException($"User - Role {request.RoleName}");

            }

            _userRepository.RemoveUser(user);
            var numberOfRows = await _unitOfWork.SaveChangesAsync();
            return numberOfRows > 1;
        }
    }
}
