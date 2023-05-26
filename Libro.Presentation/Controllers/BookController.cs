using Libro.Application.Books.Queries;
using Libro.Application.Roles.Commands;
using Libro.Domain.Entities;
using Libro.Infrastructure.Authorization;
using Libro.Presentation.Dtos.Author;
using Libro.Presentation.Dtos.Book;
using Libro.Presentation.Dtos.Role;
using Libro.Presentation.Dtos.User;
using Mapster;
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
        [HasRole("patron")]
        [HttpGet("Books", Name = "search")]
        public async Task<ActionResult<List<string>>> Search(string? Title, string? AuthorName, string? Genre, int PageNumber=0,int Count=5)
        {
            
            var query = new GetSearchedBooksQuery(Title, AuthorName, Genre, PageNumber, Count);
            var Result = await _mediator.Send(query);
            return Ok(Result);
        }
        [HasRole("patron")]
        [HttpGet("{BookId}", Name = "BookById")]
        public async Task<ActionResult<BookDto>> GetBookById(Guid BookId)
        {
            
            var query = new GetBookByIdQuery(BookId);
            var Result = await _mediator.Send(query);
            if (Result is null)
                return NotFound("Book Not_Found");
          
            return Ok(Result.Adapt<BookDto>());
        }
    }
}
