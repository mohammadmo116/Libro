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
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand,bool>
    {

        private readonly ILogger<UpdateUserCommandHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepository;

        public UpdateUserCommandHandler(
            ILogger<UpdateUserCommandHandler> logger,
              IUserRepository userRepository,
              IUnitOfWork unitOfWork
            )
        {
            _logger = logger;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetUserWtithRolesAsync(request.user.Id);

            if (user is null)
            {
                _logger.LogInformation($"CustomNotFoundException UserId:{request.user.Id}");
                throw new CustomNotFoundException("User");

            }
            if (!await _userRepository.EmailIsUniqueForUpdateAsync(user.Id, request.user.Email))
            {
                _logger.LogInformation($"UserExistsException : Email is Used");
                throw new UserExistsException(nameof(request.user.Email));
            }
            if (!await _userRepository.UserNameIsUniqueForUpdateAsync(user.Id, request.user.UserName))
            {
                _logger.LogInformation($"UserExistsException : UserName is Used");
                throw new UserExistsException(nameof(request.user.UserName));
            }
            if (!await _userRepository.PhoneNumberIsUniqueForUpdateAsync(user.Id, request.user.PhoneNumber))
            {
                _logger.LogInformation($"UserExistsException : PhoneNumber is Used");
                throw new UserExistsException(nameof(request.user.PhoneNumber));
            }

            user.UserName = request.user.UserName is null ?
                user.UserName : request.user.UserName;

            user.PhoneNumber = request.user.PhoneNumber is null ?
                user.PhoneNumber : request.user.PhoneNumber;

            user.Email = request.user.Email is null ?
                user.Email : request.user.Email;

            _userRepository.UpdateUser(user);
            var NumberOfRows = await _unitOfWork.SaveChangesAsync();
            return NumberOfRows > 0;
        }
    }
}
