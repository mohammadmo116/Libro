using Libro.Application.Books.Queries;
using Libro.Application.Roles.Commands;
using Libro.Domain.Entities;
using Libro.Presentation.Dtos.Role;
using Libro.Presentation.Dtos.User;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Presentation.Controllers
{
    [ApiController]
    [Route("Book")]
    public class BookController : ControllerBase
    {   
        private readonly IMediator _mediator;

        public BookController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpGet("Books", Name = "search")]
        public async Task<ActionResult<List<string>>> Search(string? Title, string? AuthorName, string? Genre)
        {
            
            var query = new GetSearchedBooksQuery(Title, AuthorName, Genre);
            var Result = await _mediator.Send(query);
            return Ok(Result);
        }
    }
}
