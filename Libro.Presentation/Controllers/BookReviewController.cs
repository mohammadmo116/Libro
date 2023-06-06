using Libro.Application.BookReviews.Commands;
using Libro.Domain.Entities;
using Libro.Domain.Exceptions.BookExceptions;
using Libro.Domain.Exceptions;
using Libro.Domain.Responses;
using Libro.Infrastructure.Authorization;
using Libro.Presentation.Dtos.BookReview;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using MediatR;
using Mapster;

namespace Libro.Presentation.Controllers
{
    [ApiController]
    [Route("Book")]
    public class BookReviewController : ControllerBase
    {

        private readonly IMediator _mediator;

        public BookReviewController(IMediator mediator)
        {
            _mediator = mediator;
        }

         [HasRole("patron")]
         [HttpPost("{BookId}/Review", Name = "ReviewBook")]
         public async Task<ActionResult> ReviewBook(Guid BookId, BookReviewDto bookReviewDto)
         {

             try
             {
                 string? userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid)?.Value;
                 if (!Guid.TryParse(userId, out Guid parsedUserId))
                 {
                     return BadRequest("Bad user Id");
                 }
                 var bookReview = bookReviewDto.Adapt<BookReview>();
                 bookReview.UserId = parsedUserId;
                 bookReview.BookId = BookId;
                 var query = new CreateBookReviewCommand(bookReview);
                 var Result = await _mediator.Send(query);
                 return Ok(Result.Adapt<BookReviewDto>());
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
                errorResponse.Errors?.Add(new ErrorModel() { FieldName = "BookReview", Message = e.Message });
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
