using Libro.Application.BookReviews.Commands;
using Libro.Application.BookReviews.Queries;
using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using Libro.Domain.Exceptions.BookExceptions;
using Libro.Domain.Responses;
using Libro.Infrastructure.Authorization;
using Libro.Presentation.Dtos.Author;
using Libro.Presentation.Dtos.Book;
using Libro.Presentation.Dtos.BookReview;
using Libro.Presentation.SwaggerExamples.Author;
using Libro.Presentation.SwaggerExamples.Book;
using Libro.Presentation.SwaggerExamples.BookReview;
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
    [SwaggerResponse(StatusCodes.Status403Forbidden, "When user is not Patron")]
    public class BookReviewController : ControllerBase
    {

        private readonly IMediator _mediator;

        public BookReviewController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Get Book's reviews by BookId
        /// </summary>
        /// <param name="BookId"></param>
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
        ///     GET Book/02AA22F4-66E0-45A2-9735-0DB690147662/Reviews?PageNumber=0&amp;Count=5
        /// </remarks>
        [SwaggerResponse(StatusCodes.Status200OK, "List of BookReviews with pagination", typeof(GetBookReviewsPaginationOkResultExample))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(GetBookReviewsPaginationOkResultExample))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "When Book is Not Found")]
        [HasRole("patron")]
        [HttpGet("{BookId}/Reviews", Name = "GetBookReviews")]
        public async Task<ActionResult> GetBookReviews(Guid BookId, int PageNumber = 0, int Count = 5)
        {

            try
            {
                if (Count > 10)
                    Count = 10;
                if (Count < 1)
                    Count = 1;

                var query = new GetBookReviewsQuery(BookId, PageNumber, Count);
                var Result = await _mediator.Send(query);
                return Ok(new { Reviews = Result.Item1.Adapt<List<GetBookReviewWithUserDto>>(), Pages = Result.Item2 });
            }
            catch (CustomNotFoundException e)
            {
                var errorResponse = new ErrorResponse(status: HttpStatusCode.NotFound);
                errorResponse.Errors?.Add(new ErrorModel() { FieldName = "Book", Message = e.Message });
                return new NotFoundObjectResult(errorResponse);
            }

        }
        /// <summary>
        /// add a review on a book by BookId
        /// </summary>
        /// <param name="BookId"></param>
        /// <param name="bookReviewDto"></param>
        /// <returns></returns>
        /// <remarks> 
        /// Sample request:
        ///
        ///     POST Book/02AA22F4-66E0-45A2-9735-0DB690147662/Review
        ///     {
        ///       rate: 5,
        ///       review: feedback, good 
        ///     }
        /// </remarks>
        [SwaggerResponse(StatusCodes.Status200OK, "Returns the newly created Review", Type = typeof(CreateBookReviewDto))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "When Book is Not Found")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "When Book is Not Returned or already reviewd or bad rate value",typeof(CreateBookReviewErrorResponseExample))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(CreateBookReviewErrorResponseExample))]
        [HasRole("patron")]
        [HttpPost("{BookId}/Review", Name = "ReviewBook")]
        public async Task<ActionResult> ReviewBook(Guid BookId, CreateBookReviewDto bookReviewDto)
        {

            try
            {
                string? userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
                if (!Guid.TryParse(userId, out Guid parsedUserId))
                {
                    return BadRequest("Bad user Id");
                }
                var bookReview = bookReviewDto.Adapt<BookReview>();
                bookReview.UserId = parsedUserId;
                bookReview.BookId = BookId;
                var query = new CreateBookReviewCommand(bookReview);
                var Result = await _mediator.Send(query);
                return Ok(Result.Adapt<CreateBookReviewDto>());
            }
            catch (CustomNotFoundException e)
            {
                var errorResponse = new ErrorResponse(status: HttpStatusCode.NotFound);
                errorResponse.Errors?.Add(new ErrorModel() { FieldName = "BookReview", Message = e.Message });
                return new NotFoundObjectResult(errorResponse);
            }
            catch (NotAllowedToReviewBookException e)
            {
                var errorResponse = new ErrorResponse(status: HttpStatusCode.BadRequest);
                errorResponse.Errors?.Add(new ErrorModel() { FieldName = "BookReviewNotAllowed", Message = e.Message });
                return new BadRequestObjectResult(errorResponse);
            }
            catch (BookIsAlreadyReviewedException e)
            {
                var errorResponse = new ErrorResponse(status: HttpStatusCode.BadRequest);
                errorResponse.Errors?.Add(new ErrorModel() { FieldName = "BookIsReviewed", Message = e.Message });
                return new BadRequestObjectResult(errorResponse);
            }


        }

    }
}
