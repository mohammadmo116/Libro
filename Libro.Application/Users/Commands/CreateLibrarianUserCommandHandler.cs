using Libro.Application.Interfaces;
using Libro.Domain.Entities;
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
    public class CreateLibrarianUserCommandHandler : IRequestHandler<CreateLibrarianUserCommand, User>
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly ILogger<CreateLibrarianUserCommandHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;
        public CreateLibrarianUserCommandHandler(ILogger<CreateLibrarianUserCommandHandler> logger,
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _roleRepository= roleRepository;
        }
        public async Task<User> Handle(CreateLibrarianUserCommand request, CancellationToken cancellationToken)
        {

           var role = await _roleRepository.GetRoleByNameAsync("librarian".ToLower());

            if (role is null)
            {
                _logger.LogInformation($"CustomNotFoundException Role : librarian");
                throw new CustomNotFoundException("Role");
            }
          

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

            UserRole userRole = new()
            {
                RoleId = role.Id,
                UserId = request.User.Id

            };
            var user = await _userRepository.RegisterUserAsync(request.User);
            await _userRepository.AssignRoleToUserAsync(userRole);
            await _unitOfWork.SaveChangesAsync();
            return user;
        }
    }
}
