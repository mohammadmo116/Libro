using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Application.Books.Queries
{
    public sealed record class GetSearchedBooksQuery(string? Title, string? AuthorName, string? Genre) : IRequest<List<string>>;


}
