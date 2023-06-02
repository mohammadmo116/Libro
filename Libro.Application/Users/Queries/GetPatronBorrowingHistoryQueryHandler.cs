using Libro.Application.Interfaces;
using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Application.Users.Queries
{
    public sealed class GetPatronBorrowingHistoryQueryHandler:IRequestHandler<GetPatronBorrowingHistoryQuery,List<BookTransaction>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<GetPatronBorrowingHistoryQueryHandler> _logger;

        public GetPatronBorrowingHistoryQueryHandler(
            ILogger<GetPatronBorrowingHistoryQueryHandler> logger,
            IUserRepository userRepository
            )
        {
            _logger = logger;
            _userRepository = userRepository;

        }

        public async Task<List<BookTransaction>> Handle(GetPatronBorrowingHistoryQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
