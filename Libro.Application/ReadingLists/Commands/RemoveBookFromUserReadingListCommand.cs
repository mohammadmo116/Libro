using Libro.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Application.ReadingLists.Commands
{
     public sealed record RemoveBookFromUserReadingListCommand(Guid UserId, BookReadingList BookReadingList) 
                : IRequest<bool>;
}
