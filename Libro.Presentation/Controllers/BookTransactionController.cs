using FluentValidation.Validators;
using Libro.Application.Books.Queries;
using Libro.Application.BookTransactions.Commands;
using Libro.Application.BookTransactions.Queiries;
using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using Libro.Domain.Responses;
using Libro.Infrastructure.Authorization;
using Libro.Presentation.Dtos.BookTransaction;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Presentation.Controllers
{
    [ApiController]
    [Route("BookTransaction")]
    public class BookTransactionController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BookTransactionController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HasRole("patron")]
        [HttpPost("Reserve")]
        public async Task<ActionResult> ReserveBook(ReserveBookTransactionDto reserveBookDto)
        {

            try
            {
                var bookTransaction = reserveBookDto.Adapt<BookTransaction>();
                var query = new ReserveBookCommand(bookTransaction);
                var Result = await _mediator.Send(query);
                return Result ? Ok("Book has been reserved") : BadRequest();
            }
            catch (CustomNotFoundException e) {
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
        [HttpPut("Borrow/{TransactionId}")]
        public async Task<ActionResult> CheckOutBook(Guid TransactionId, DueDateDto dueDateDto)
        {   
            try
            {
                var query = new CheckOutBookCommand(TransactionId, dueDateDto.DueDate);
                var result =  await _mediator.Send(query);
                return result? Ok("Book has been Borrowed") : BadRequest();
            }
            catch (CustomNotFoundException e)
            {
                var errorResponse = new ErrorResponse(status: HttpStatusCode.NotFound);
                errorResponse.Errors?.Add(new ErrorModel() { FieldName = "Book", Message = e.Message });
                return new NotFoundObjectResult(errorResponse);
            }

            catch(BookIsBorrowedException e)
            {
                var errorResponse = new ErrorResponse(status: HttpStatusCode.BadRequest);
                errorResponse.Errors?.Add(new ErrorModel() { FieldName = "Book", Message = e.Message });
                return new BadRequestObjectResult(errorResponse);

            }
            
        }

        [HasRole("librarian")]
        [HttpPut("Return/{TransactionId}")]
        public async Task<ActionResult> ReturnBook(Guid TransactionId)
        {
            try
            {
                var query = new ReturnBookCommand(TransactionId);
                var result = await _mediator.Send(query);
                return result? Ok("Book has been Returned") : BadRequest();
            }
            catch (CustomNotFoundException e)
            {
                var errorResponse = new ErrorResponse(status: HttpStatusCode.NotFound);
                errorResponse.Errors?.Add(new ErrorModel() { FieldName = "Book", Message = e.Message });
                return new NotFoundObjectResult(errorResponse);
            }

        }

        [HasRole("librarian")]
        [HttpPut("TrackTransaction")]
        public async Task<ActionResult> TrackDueDate(int PageNumber = 0, int Count = 5)
        {
            if (Count > 10)
                 Count = 10;
                var query = new TrackDueDateQuery(PageNumber, Count);
                var Result = await _mediator.Send(query);
                return Ok(Result.Adapt<List<BookTransactionDto>>());
        }
    }
}
