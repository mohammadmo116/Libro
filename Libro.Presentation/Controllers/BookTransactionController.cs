using Libro.Application.Books.Queries;
using Libro.Application.BookTransactions.Commands;
using Libro.Application.BookTransactions.Queiries;
using Libro.Application.BookTransactions.Queries;
using Libro.Domain.Entities;
using Libro.Domain.Enums;
using Libro.Domain.Exceptions;
using Libro.Domain.Exceptions.BookExceptions;
using Libro.Domain.Responses;
using Libro.Infrastructure.Authorization;
using Libro.Presentation.Dtos.Book;
using Libro.Presentation.Dtos.BookTransaction;
using Libro.Presentation.SwaggerExamples.Book;
using Libro.Presentation.SwaggerExamples.BookReview;
using Libro.Presentation.SwaggerExamples.BookTransaction;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using System.Net;
using System.Security.Claims;

namespace Libro.Presentation.Controllers
{
    [ApiController]
    [Route("Book")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authorization has been denied for this request")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "When user is not librarian")]
    public class BookTransactionController : ControllerBase
    {

        private readonly IMediator _mediator;

        public BookTransactionController(IMediator mediator)
        {
            _mediator = mediator;
        }



        /// <summary>
        /// Get Book Transaction By Transaction Id
        /// </summary>
        /// <param name="TransactionId"></param>
        /// <returns></returns>
        /// <remarks> 
        /// Sample request:
        /// 
        ///     GET /Book/Transactions/455AE5E6-D447-4A66-AB59-5E48B3E69E03  
        /// </remarks>
        [SwaggerResponse(StatusCodes.Status200OK, description: "Returns BookTransaction Details", Type = typeof(BookTransactionWithStatusAndIdDto))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "When BookTransaction Is Not Found")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "When user is not Admin,Librarian or Patron")]
        [HasRole("admin, librarian, patron")]
        [HttpGet("Transactions/{TransactionId}", Name = "GetBookTransactionById")]
        public async Task<ActionResult<BookTransactionWithStatusAndIdDto>> GetBookTransactionById(Guid TransactionId)
        {
            try {
                string? userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
                _ = Guid.TryParse(userId, out Guid parsedUserId);
                
                var query = new GetBookTransactionQuery(parsedUserId,TransactionId);
            var Result = await _mediator.Send(query);
            return Ok(Result.Adapt<BookTransactionWithStatusAndIdDto>());
            }
            catch(CustomNotFoundException e) {
                var errorResponse = new ErrorResponse(status: HttpStatusCode.NotFound);
                errorResponse.Errors?.Add(new ErrorModel() { FieldName = e._field, Message = e.Message });
                return new NotFoundObjectResult(errorResponse);

            }
        }


        /// <summary>
        /// Reserve book by BookId
        /// </summary>
        /// <param name="BookId"></param>
        /// <returns></returns>
        /// <remarks> 
        /// Sample request:
        ///
        ///     POST Book/02AA22F4-66E0-45A2-9735-0DB690147662/Reserve
        /// </remarks>
        [SwaggerResponse(StatusCodes.Status200OK, "Success when book is Reserved")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "When Book is Not Found")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "When Book is Not not available", typeof(ReserveBookErrorResponseExample))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(ReserveBookErrorResponseExample))]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "When user is not patron")]
        [HasRole("patron")]
        [HttpPost("{BookId}/Reserve", Name = "ReserveBook")]  
        public async Task<ActionResult> ReserveBook(Guid BookId)
        {

            try
            {
                string? userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
                _ = Guid.TryParse(userId, out Guid parsedUserId);
                
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
        /// <summary>
        /// Checkout/Borrow a reserved book for patron transactionId
        /// </summary>
        /// <param name="TransactionId"></param>
        /// <param name="dueDateDto"></param>
        /// <returns></returns>
        /// <remarks> 
        /// Sample request:
        ///
        ///     PUT Book/Transactions/BBAB7E04-92FD-42D0-BB67-050379BE5585/Borrow
        /// </remarks>
        [SwaggerResponse(StatusCodes.Status200OK, "Success when book is Borrowed")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "When Transaction is Not Found")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "When Book is Already Borrowed", typeof(CheckOutBookErrorResponseExample))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(CheckOutBookErrorResponseExample))]
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
                errorResponse.Errors?.Add(new ErrorModel() { FieldName = "bookTransaction", Message = e.Message });
                return new NotFoundObjectResult(errorResponse);
            }

            catch (BookIsBorrowedException e)
            {
                var errorResponse = new ErrorResponse(status: HttpStatusCode.BadRequest);
                errorResponse.Errors?.Add(new ErrorModel() { FieldName = "Book", Message = e.Message });
                return new BadRequestObjectResult(errorResponse);

            }

        }
        /// <summary>
        /// Return book by transactionId
        /// </summary>
        /// <param name="TransactionId"></param>
        /// <returns></returns>
        /// <remarks> 
        /// Sample request:
        ///
        ///     PUT Book/Transactions/BBAB7E04-92FD-42D0-BB67-050379BE5585/Return
        /// </remarks>
        [SwaggerResponse(StatusCodes.Status200OK, "Success when book is Returned")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "When Transaction is Not Found")]
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
                errorResponse.Errors?.Add(new ErrorModel() { FieldName = "bookTransaction", Message = e.Message });
                return new NotFoundObjectResult(errorResponse);
            }

        }



        /// <summary>
        /// get all transactions for the borrowed books with due dates with pagination
        /// </summary>
        /// <param name="PageNumber"></param>
        /// <param name="Count"></param>
        /// <returns></returns>
        /// <remarks> 
        /// Route Defualts:
        ///  
        ///     { 
        ///     Defualt:
        ///         PageNumber=0,
        ///         Count=5
        ///     
        ///     Max:
        ///         Count=10
        ///     }
        /// Sample request:
        ///
        ///     GET /Book/Due-Date-Transactions?PageNumber=0&amp;Count=5
        /// </remarks>
        [SwaggerResponse(StatusCodes.Status200OK, "List of Transactions with pagination", typeof(GetTransactionsPaginationOkResultExample))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(GetTransactionsPaginationOkResultExample))]
        [HasRole("librarian")]
        [HttpGet("Due-Date-Transactions")]
        public async Task<ActionResult<(List<BookTransactionWithStatusAndIdDto>, int)>> TrackDueDate(int PageNumber = 0, int Count = 5)
        {
            if (Count > 10)
                Count = 10;
            if (Count < 1)
                Count = 1;
            var query = new TrackDueDateQuery(PageNumber, Count);
            var Result = await _mediator.Send(query);


            return Ok(new
            {
                Transactions = Result.Item1.Adapt<List<BookTransactionWithStatusAndIdDto>>()
                ,
                Pages = Result.Item2
            });
        }
    }
}
