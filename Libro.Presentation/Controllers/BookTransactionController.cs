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
using System.Security.Claims;
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
       


        [HasRole("librarian")]
        [HttpPut("{TransactionId}/Return")]
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
        [HttpPut("Transactions")]
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
