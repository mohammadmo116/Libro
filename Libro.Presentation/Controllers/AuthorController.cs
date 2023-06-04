using Libro.Application.Authors.Commands;
using Libro.Application.Authors.Queries;
using Libro.Application.Books.Commands;
using Libro.Application.Books.Queries;
using Libro.Domain.Entities;
using Libro.Infrastructure.Authorization;
using Libro.Presentation.Dtos.Author;
using Libro.Presentation.Dtos.Book;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Presentation.Controllers
{
    [ApiController]
    [Route("Author")]
    public class AuthorController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthorController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize()]
        [HttpGet("{AuthorId}", Name = "GetAuthor")]
        public async Task<ActionResult> GetAuthor(Guid AuthorId)
        {

            var query = new GetAuthorQuery(AuthorId);
            var Result = await _mediator.Send(query);
            if (Result is null)
                return NotFound("Author Not_Found");

            return Ok(Result.Adapt<AuthorDto>());

        }

        [HasRole("librarian")]
        [HttpPost(Name = "CreateAuthor")]
        public async Task<ActionResult> CreateAuthor(CreateAuthorDto authorDto)
        {
            var author = authorDto.Adapt<Author>();
            var command = new CreateAuthorCommand(author);
            var Result = await _mediator.Send(command);
            var ResultAuthorDto = Result.Adapt<AuthorDto>();
            return CreatedAtAction(nameof(GetAuthor), new { BookId = ResultAuthorDto.Id }, ResultAuthorDto);



        }

    }
}
