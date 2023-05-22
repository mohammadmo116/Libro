using Libro.Application.Interfaces;
using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Libro.Application.Roles.Commands
{
    public sealed class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand,Role>
    {
        private readonly IRoleRepository _roleRepository;
        private readonly ILogger<Role> _logger;
        public CreateRoleCommandHandler(IRoleRepository roleRepository,
                                        ILogger<Role> logger)
        {
            _roleRepository = roleRepository;
            _logger = logger;
        }

       
       

        public async Task<Role> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            try { 
            var role=await _roleRepository.AddRoleAsync(request.Role);
            return role;
            }
            catch(RoleExistsException e) {
                throw e;
                    }
        }
    }
}
