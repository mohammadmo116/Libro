﻿using Libro.Application.Interfaces;
using Libro.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Application.Users.Queries
{
    public class GetUserQueryHandler : IRequestHandler<GetUserQuery, User>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<GetUserQueryHandler> _logger;

        public GetUserQueryHandler(
            ILogger<GetUserQueryHandler> logger,
            IUserRepository userRepository
            )
        {
            _logger = logger;
            _userRepository = userRepository;

        }
        public async Task<User> Handle(GetUserQuery request, CancellationToken cancellationToken)
        {
            
            throw new NotImplementedException();
        }
    }
}
