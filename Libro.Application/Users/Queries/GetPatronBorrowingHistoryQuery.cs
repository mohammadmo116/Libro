﻿using Libro.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Application.Users.Queries
{
    public sealed record GetPatronBorrowingHistoryQuery(Guid PatronId, int PageNumber, int Count) : IRequest<List<BookTransaction>>;
   
}
