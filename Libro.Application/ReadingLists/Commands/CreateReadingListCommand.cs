﻿using Libro.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Application.ReadingLists.Commands
{
    public sealed record class CreateReadingListCommand(Guid UserId, ReadingList ReadingList):IRequest<ReadingList>;
    
}
