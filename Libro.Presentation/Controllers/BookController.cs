using Libro.Application.Books.Queries;
using Libro.Application.BookTransactions.Commands;
using Libro.Application.Roles.Commands;
using Libro.Domain.Entities;
using Libro.Domain.Enums;
using Libro.Domain.Exceptions;
using Libro.Domain.Responses;
using Libro.Infrastructure.Authorization;
using Libro.Presentation.Dtos.Author;
using Libro.Presentation.Dtos.Book;
using Libro.Presentation.Dtos.BookTransaction;
using Libro.Presentation.Dtos.Role;
using Libro.Presentation.Dtos.User;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
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
        [HttpGet("", Name = "search")]
        public async Task<ActionResult<List<string>>> Search(string? Title, string? AuthorName, string? Genre, int PageNumber = 0, int Count = 5)
        {
            if (Count > 10)
                Count = 10;
            var query = new GetSearchedBooksQuery(Title, AuthorName, Genre, PageNumber, Count);
            var Result = await _mediator.Send(query);
            return Ok(Result);
        }

        [HasRole("patron")]
        [HttpGet("{BookId}", Name = "BookById")]
        public async Task<ActionResult<BookWithAuthorsDto>> GetBookById(Guid BookId)
        {

            var query = new GetBookByIdQuery(BookId);
            var Result = await _mediator.Send(query);
            if (Result is null)
                return NotFound("Book Not_Found");

            return Ok(Result.Adapt<BookWithAuthorsDto>());
        }
      
       
        [HasRole("patron")]
        [HttpPost("{BookId}/Reserve", Name = "ReserveBook")]
        public async Task<ActionResult> ReserveBook(Guid BookId)
        {

            try
            {
                string? userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid)?.Value;
                if (!Guid.TryParse(userId, out Guid parsedUserId))
                {
                    return BadRequest("Bad user Id");
                }
                BookTransaction bookTransaction = new()
                {
                    Id = Guid.NewGuid(),
                    UserId = parsedUserId,
                    BookId = BookId,
                    Status = BookStatus.Reserved

                };
                var query = new ReserveBookCommand(bookTransaction);
                var Result = await _mediator.Send(query);
                return Result ? Ok("Book has been reserved") : BadRequest();
            }
            catch (CustomNotFoundException e)
            {
                var errorResponse = new ErrorResponse(status: HttpStatusCode.NotFound);
                errorResponse.Errors?.Add(new ErrorModel() { FieldName = "Book", Message = e.Message });
                return new NotFoundObjectResult(errorResponse);
            }
            catch (BookIsNotAvailableException e)
            {
                var errorResponse = new ErrorResponse(status: HttpStatusCode.BadRequest);
                errorResponse.Errors?.Add(new ErrorModel() { FieldName = "Book", Message = e.Message });
                return new BadRequestObjectResult(errorResponse);
            }

        }

    }
}
