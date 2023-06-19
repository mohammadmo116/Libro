﻿using Libro.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Application.ReadingLists.Queries
{
    public sealed record class GetUserReadingListsQuery(Guid UserId, int PageNumber, int Count) :IRequest<(List<ReadingList>,int)>;
}