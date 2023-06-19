using Libro.Application.Books.Commands;
using Libro.Application.Books.Queries;
using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using Libro.Domain.Responses;
using Libro.Infrastructure.Authorization;
using Libro.Presentation.Dtos.Book;
using Libro.Presentation.SwaggerExamples.Book;
using Libro.Presentation.SwaggerExamples.Patron;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using System.Net;
using System.Reflection;

namespace Libro.Presentation.Controllers
{
    [ApiController]
    [Route("Book")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authorization has been denied for this request")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "When user is not Librarian")]
    public class BookController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BookController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Search Books with optional Params To get the paginated Books
        /// </summary>
        /// <param name="Title"></param>
        /// <param name="AuthorName"></param>
        /// <param name="Genre"></param>
        /// <param name="PageNumber"></param>
        /// <param name="Count"></param>
        /// <returns>List of books</returns>
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
        ///     GET /Book?PageNumber=0&amp;Count=5
        ///     {
        ///        
        ///     }
        /// sample request2:
        ///
        ///     GET /Book?PageNumber=1&amp;Count=7
        ///     {
        ///        Title : "test"
        ///     }
        /// </remarks>
        [SwaggerResponse(StatusCodes.Status200OK, "List of Books with pagination", typeof(GetBookPaginationOkResultExample))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(GetBookPaginationOkResultExample))]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "When user is not Admin,Librarian or Patron")]
        [HasRole("librarian,admin,patron")]
        [HttpGet("", Name = "search")]
        public async Task<ActionResult<(List<GetBookDto>, int)>> Search(string? Title, string? AuthorName, string? Genre, int PageNumber = 0, int Count = 5)
        {
            if (Count > 10)
                Count = 10;
            if (Count < 1)
                Count = 1;
            var query = new GetSearchedBooksQuery(Title, AuthorName, Genre, PageNumber, Count);
            var Result = await _mediator.Send(query);

            return Ok(new { Books = Result.Item1.Adapt<List<GetBookDto>>(), Pages = Result.Item2 });
        }

        /// <summary>
        /// Get a specific book Details With the Authors by BookId
        /// </summary>
        /// <param name="BookId"></param>
        /// <returns>The Book Details With the Authors</returns>
        /// <remarks> 
        /// Sample request:
        /// 
        ///     GET /Book/02AA22F4-66E0-45A2-9735-0DB690147662  
        /// </remarks>
        [SwaggerResponse(StatusCodes.Status200OK, description: "Returns Book Detailts With Authors", Type = typeof(List<BookWithAuthorsDto>))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "When Book Is Not Found")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "When user is not Admin,Librarian or Patron")]
        [HasRole("librarian,admin,patron")]
        [HttpGet("{BookId}", Name = "GetBookById")]
        public async Task<ActionResult<BookWithAuthorsDto>> GetBookById(Guid BookId)
        {

            var query = new GetBookByIdQuery(BookId);
            var Result = await _mediator.Send(query);
            if (Result is null)
            {
                var errorResponse = new ErrorResponse(status: HttpStatusCode.NotFound);
                errorResponse.Errors?.Add(new ErrorModel() { FieldName = "Book", Message = "404 Book NotFound" });
                return new NotFoundObjectResult(errorResponse);
            }
          

            return Ok(Result.Adapt<BookWithAuthorsDto>());
        }


        /// <summary>
        /// Create New Book
        /// </summary>
        /// <param name="bookDto"></param>
        /// <returns>Returns the newly created item and it's route in the header:location</returns>
        /// <remarks> 
        /// Sample request:
        /// 
        ///     POST /Book
        ///     {
        ///         title: string,
        ///         genre: string,
        ///         publishedDate: "2023-06-16T09:09:56.356Z",
        ///         isAvailable: true
        ///     }
        /// </remarks>
       
        [SwaggerResponse(StatusCodes.Status201Created, "Returns the newly created Book and it's route in the header:location", Type = typeof(BookWithAuthorsDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest,"when PublishedDate is in the future", typeof(CreateBookErrorResponseExample))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(CreateBookErrorResponseExample))]
        [HasRole("librarian,patron")]
        [HttpPost(Name = "CreateBook")]
        public async Task<CreatedAtActionResult> CreateBook(CreateBookDto bookDto)
        {
            var book = bookDto.Adapt<Book>();
            var command = new CreateBookCommand(book);
            var Result = await _mediator.Send(command);
            var bookWithAuthorsDto = Result.Adapt<BookWithAuthorsDto>();
            return CreatedAtAction(nameof(GetBookById), new { BookId = bookWithAuthorsDto.Id }, bookWithAuthorsDto);

        }


        /// <summary>
        /// Update Book by Id
        /// </summary>
        /// <param name="BookId"></param>
        /// <param name="bookDto"></param>
        /// <returns></returns>
        /// <remarks> 
        /// Sample request:
        /// 
        ///     PUT /Book/02AA22F4-66E0-45A2-9735-0DB690147662 
        ///     {
        ///         id: 02AA22F4-66E0-45A2-9735-0DB690147662,
        ///         title: string,
        ///         genre: string,
        ///         publishedDate: 2023-06-16T15:06:59.064Z,
        ///         isAvailable: true
        ///     }
        /// </remarks>
        [SwaggerResponse(StatusCodes.Status200OK, "Success when book is updated")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "When Book is Not Found")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "When BookId in the route does not match the one in the body or PublishedDate is in the future", typeof(UpdateBookErrorResponseExample))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(UpdateBookErrorResponseExample))]
        [HasRole("librarian")]
        [HttpPut("{BookId}", Name = "UpdateBook")]     
        public async Task<ActionResult<bool>> UpdateBook(Guid BookId, UpdateBookDto bookDto)
        {
            try
            {
                if (BookId != bookDto.Id)
                {
                    var errorResponse = new ErrorResponse(status: HttpStatusCode.BadRequest);
                    errorResponse.Errors?.Add(new ErrorModel() { FieldName = "Id", Message = "bad Id"});
                    return new BadRequestObjectResult(errorResponse);
                }
                var book = bookDto.Adapt<Book>();
                var command = new UpdateBookCommand(book);
                var Result = await _mediator.Send(command);
                return Result ? Ok("Book has been Updated") : StatusCode(StatusCodes.Status500InternalServerError);
            }
            catch (CustomNotFoundException e)
            {
                var errorResponse = new ErrorResponse(status: HttpStatusCode.NotFound);
                errorResponse.Errors?.Add(new ErrorModel() { FieldName = "Book", Message = e.Message });
                return new NotFoundObjectResult(errorResponse);

            }
        }

        /// <summary>
        /// Remove Book By Id
        /// </summary>
        /// <param name="BookId"></param>
        /// <returns></returns>
        /// <remarks> 
        /// Sample request:
        /// 
        ///     DELETE /Book/02AA22F4-66E0-45A2-9735-0DB690147662 
        /// </remarks>
        [SwaggerResponse(StatusCodes.Status200OK, "Returns Success when book is deleted")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "When Book is Not Found")]
        [HasRole("librarian")]
        [HttpDelete("{BookId}", Name = "RemoveBook")]
        public async Task<ActionResult<bool>> RemoveBook(Guid BookId)
        {
            try
            {

                var command = new RemoveBookCommand(BookId);
                var Result = await _mediator.Send(command);
                return Result ? Ok("Book has been Deleted") : StatusCode(StatusCodes.Status500InternalServerError);
            }
            catch (CustomNotFoundException e)
            {
                var errorResponse = new ErrorResponse(status: HttpStatusCode.NotFound);
                errorResponse.Errors?.Add(new ErrorModel() { FieldName = "Book", Message = e.Message });
                return new BadRequestObjectResult(errorResponse);

            }
        }





    }
}
