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
    public sealed class UpdatePatronUserCommandHandler : IRequestHandler<UpdatePatronUserCommand, bool>
    {
        private readonly ILogger<UpdatePatronUserCommandHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepository;

        public UpdatePatronUserCommandHandler(
            ILogger<UpdatePatronUserCommandHandler> logger,
              IUserRepository userRepository,
              IUnitOfWork unitOfWork
            ) 
        {
            _logger = logger;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(UpdatePatronUserCommand request, CancellationToken cancellationToken)
        {

            var user = await _userRepository.GetUserWtithRolesAsync(request.user.Id);

            if (user is null || !user.Roles.Any(r => r.Name.ToLower() == "patron"))
            {
                _logger.LogInformation($"CustomNotFoundException UserId:{request.user.Id}");
                throw new CustomNotFoundException("User");

            }
            user.UserName = request.user.UserName is null? 
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
