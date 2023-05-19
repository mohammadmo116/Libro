using Libro.Application.Interfaces;
using Libro.Domain.Entities;
using MediatR;


namespace Libro.Application.Users.Commands
{

    public sealed class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, User>
    {
        private readonly IAuthenticationRepository _authenticationRepository;

        public CreateUserCommandHandler(IAuthenticationRepository authenticationRepository)
        {
            _authenticationRepository = authenticationRepository;
        }

        public Task<User> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            
            return _authenticationRepository.RegisterUser(request.User);
        }
    }

}
