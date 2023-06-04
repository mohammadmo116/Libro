using Libro.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Application.Authors.Queries
{
    public sealed record GetAuthorQuery(Guid AuthorId):IRequest<Author>;
    
}
