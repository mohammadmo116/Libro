using Libro.Application.Authors.Commands;
using Libro.Application.Authors.Queries;
using Libro.Application.Books.Commands;
using Libro.Application.Books.Queries;
using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using Libro.Domain.Responses;
using Libro.Infrastructure.Authorization;
using Libro.Presentation.Dtos.Author;
using Libro.Presentation.Dtos.Book;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
            return CreatedAtAction(nameof(GetAuthor), new { AuthorId = ResultAuthorDto.Id }, ResultAuthorDto);

        }

        [HasRole("librarian")]
        [HttpPut("{AuthorId}", Name = "UpdateAuthor")]
        public async Task<ActionResult> UpdateAuthor(Guid AuthorId, AuthorDto authorDto)
        {
            try
            {
                if (AuthorId != authorDto.Id)
                {
                    return BadRequest("bad Id");
                }
                var author = authorDto.Adapt<Author>();
                var command = new UpdateAuthorCommand(author);
                var Result = await _mediator.Send(command);
                return Result ? Ok("Author has been Updated") : StatusCode(StatusCodes.Status500InternalServerError);
            }
            catch (CustomNotFoundException e)
            {
                var errorResponse = new ErrorResponse(status: HttpStatusCode.NotFound);
                errorResponse.Errors?.Add(new ErrorModel() { FieldName = "Author", Message = e.Message });
                return new BadRequestObjectResult(errorResponse);

            }
        }

        [HasRole("librarian")]
        [HttpDelete("{AuthorId}", Name = "RemoveAuthor")]
        public async Task<ActionResult> RemoveAuthor(Guid AuthorId)
        {
            try
            {

                var command = new RemoveAuthorCommand(AuthorId);
                var Result = await _mediator.Send(command);
                return Result ? Ok("Author has been Deleted") : StatusCode(StatusCodes.Status500InternalServerError);
            }
            catch (CustomNotFoundException e)
            {
                var errorResponse = new ErrorResponse(status: HttpStatusCode.NotFound);
                errorResponse.Errors?.Add(new ErrorModel() { FieldName = "Author", Message = e.Message });
                return new BadRequestObjectResult(errorResponse);

            }
        }

    }
}
