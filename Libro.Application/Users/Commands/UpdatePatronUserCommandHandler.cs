using Libro.Application.Interfaces;
using Libro.Domain.Exceptions;
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

        private readonly IUserRepository _userRepository;

        public UpdatePatronUserCommandHandler(
            ILogger<UpdatePatronUserCommandHandler> logger,
              IUserRepository userRepository
            ) 
        {
            _logger = logger;
            _userRepository = userRepository;
        }

        public async Task<bool> Handle(UpdatePatronUserCommand request, CancellationToken cancellationToken)
        {
            
            throw new NotImplementedException();

        }
    }
}
