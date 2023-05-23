﻿using Libro.Application.Interfaces;
using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Libro.Application.Users.Commands
{

    public sealed class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, User>
    {
        private readonly IAuthenticationRepository _authenticationRepository;
        private readonly ILogger<User> _logger;

        public CreateUserCommandHandler(ILogger<User> logger,
            IAuthenticationRepository authenticationRepository)
        {
            _authenticationRepository = authenticationRepository;
            _logger= logger;
        }

        public async Task<User> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                
                var user = await _authenticationRepository.RegisterUserAsync(request.User);
                return user;
            }
            catch (UserExistsException e)
            {    
                throw new UserExistsException(e._field); 
            }

        }
    }

}