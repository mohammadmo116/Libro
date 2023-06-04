using Libro.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Application.Authors.Commands
{
    public sealed record  CreateAuthorCommand(Author Author):IRequest<Author>;
    
}
