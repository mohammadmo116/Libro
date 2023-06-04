using Libro.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Application.Books.Commands
{
    public sealed record UpdateBookCommand(Book Book):IRequest<bool>;
   
}
