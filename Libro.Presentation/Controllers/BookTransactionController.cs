using Libro.Application.BookTransactions.Commands;
using Libro.Application.BookTransactions.Queiries;
using Libro.Domain.Exceptions.BookExceptions;
using Libro.Domain.Exceptions;
using Libro.Domain.Responses;
using Libro.Infrastructure.Authorization;
using Libro.Presentation.Dtos.BookTransaction;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Libro.Domain.Enums;
using System.Security.Claims;
using Libro.Domain.Entities;
using Mapster;

namespace Libro.Presentation.Controllers
{
    [ApiController]
    [Route("Book")]
    public class BookTransactionController : ControllerBase
    {

        private readonly IMediator _mediator;

        public BookTransactionController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HasRole("patron")]
        [HttpPost("{BookId}/Reserve", Name = "ReserveBook")]
        public async Task<ActionResult> ReserveBook(Guid BookId)
        {

            try
            {
                string? userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
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
                return Result ? Ok("Book has been reserved") : StatusCode(StatusCodes.Status500InternalServerError);
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

        [HasRole("librarian")]
        [HttpPut("Transactions/{TransactionId}/Borrow", Name = "BorrowBook")]
        public async Task<ActionResult> CheckOutBook(Guid TransactionId, DueDateDto dueDateDto)
        {
            try
            {

                var query = new CheckOutBookCommand(TransactionId, dueDateDto.DueDate);
                var result = await _mediator.Send(query);
                return result ? Ok("Book has been Borrowed") : StatusCode(StatusCodes.Status500InternalServerError);
            }
            catch (CustomNotFoundException e)
            {
                var errorResponse = new ErrorResponse(status: HttpStatusCode.NotFound);
                errorResponse.Errors?.Add(new ErrorModel() { FieldName = "Book", Message = e.Message });
                return new NotFoundObjectResult(errorResponse);
            }

            catch (BookIsBorrowedException e)
            {
                var errorResponse = new ErrorResponse(status: HttpStatusCode.BadRequest);
                errorResponse.Errors?.Add(new ErrorModel() { FieldName = "Book", Message = e.Message });
                return new BadRequestObjectResult(errorResponse);

            }

        }

        [HasRole("librarian")]
        [HttpPut("Transactions/{TransactionId}/Return", Name = "ReturnBook")]
        public async Task<ActionResult> ReturnBook(Guid TransactionId)
        {
            try
            {
                var query = new ReturnBookCommand(TransactionId);
                var result = await _mediator.Send(query);
                return result ? Ok("Book has been Returned") : StatusCode(StatusCodes.Status500InternalServerError);
            }
            catch (CustomNotFoundException e)
            {
                var errorResponse = new ErrorResponse(status: HttpStatusCode.NotFound);
                errorResponse.Errors?.Add(new ErrorModel() { FieldName = "Book", Message = e.Message });
                return new NotFoundObjectResult(errorResponse);
            }

        }

       


        [HasRole("librarian")]
        [HttpGet("Transactions")]
        public async Task<ActionResult<(List<BookTransactionWithStatusDto>, int)>> TrackDueDate(int PageNumber = 0, int Count = 5)
        {
            if (Count > 10)
                Count = 10;
            if (Count < 1)
                Count = 1;
            var query = new TrackDueDateQuery(PageNumber, Count);
            var Result = await _mediator.Send(query);


            return Ok(new
            {
                Books = Result.Item1.Adapt<List<BookTransactionWithStatusDto>>()
                ,
                Pages = Result.Item2
            });
        }
    }
}
