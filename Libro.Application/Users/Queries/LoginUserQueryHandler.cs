using Libro.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Security.Authentication;


namespace Libro.Application.Users.Queries
{
    public sealed class LoginUserQueryHandler : IRequestHandler<LoginUserQuery, string>
    {
        private readonly IAuthenticationRepository _authenticationRepository;
        private readonly ILogger<LoginUserQueryHandler> _logger;

        public LoginUserQueryHandler(ILogger<LoginUserQueryHandler> logger,
            IAuthenticationRepository authenticationRepository)
        {
            _authenticationRepository = authenticationRepository;
            _logger = logger;
        }
        public async Task<string> Handle(LoginUserQuery query, CancellationToken cancellationToken)
        {

            var user = await _authenticationRepository.ValidateUserCredentialsAsync(query.Email, query.Password);
            if (user is null)
            {
                throwInvalidCredentialException(query);
            }
            var jwt = await _authenticationRepository.Authenticate(user);
            if (jwt is null)
            {
                throwInvalidCredentialException(query);
            }
            return jwt;



        }
        private void throwInvalidCredentialException(LoginUserQuery query)
        {
            var message = $"Invalid Credentials,\n email : {query.Email} \n password: {query.Password}";
            _logger.LogInformation(message);
            throw new InvalidCredentialException(message);
        }
    }
}
