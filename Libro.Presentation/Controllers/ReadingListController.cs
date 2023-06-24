using Libro.Application.ReadingLists.Commands;
using Libro.Application.ReadingLists.Queries;
using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using Libro.Domain.Exceptions.ReadingListExceptions;
using Libro.Domain.Responses;
using Libro.Infrastructure.Authorization;
using Libro.Presentation.Dtos.ReadingList;
using Libro.Presentation.SwaggerExamples.ReadingList;
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
    [Route("ReadingList")]
    [ApiController]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authorization has been denied for this request")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "When user is not Patron")]
    public class ReadingListController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReadingListController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get ReadingLists
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
        ///     GET /ReadingList?PageNumber=0&amp;Count=5
        /// </remarks>
        [SwaggerResponse(StatusCodes.Status200OK, "List of ReadingLists with pagination", typeof(GetReadingListPaginationOkResultExample))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(GetReadingListPaginationOkResultExample))]
        [HasRole("patron")]
        [HttpGet(Name = "GetReadingLists")]
        public async Task<ActionResult<(List<GetReadingListDto>, int)>> GetReadingLists(int PageNumber = 0, int Count = 5)
        {
            if (Count > 10)
                Count = 10;
            if (Count < 1)
                Count = 1;

            string? userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            _ = Guid.TryParse(userId, out Guid parsedUserId);

            var query = new GetUserReadingListsQuery(parsedUserId, PageNumber, Count);
            var Result = await _mediator.Send(query);

            return Ok(new
            {
                ReadingLists = Result.Item1.Adapt<List<GetReadingListDto>>(),
                Pages = Result.Item2
            }
            );
        }


        /// <summary>
        /// Get ReadingList with it's books with pagination
        /// </summary>
        /// <param name="ReadingListId"></param>
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
        ///     GET /ReadingList/D9BBC76C-AFBB-4AB0-BE01-ADCCD95C38D8/Books?PageNumber=0&amp;Count=5
        /// </remarks>
        [SwaggerResponse(StatusCodes.Status200OK, "List of readlingList with it's Books - pagination", typeof(GetReadingListWithBooksPagonationOkResultExample))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(GetReadingListWithBooksPagonationOkResultExample))]
        [HasRole("patron")]
        [HttpGet("{ReadingListId}/Books", Name = "GetReadingListWithBooks")]
        public async Task<ActionResult<(GetReadingListWithBooksDto, int)>> GetReadingListWithBooks(Guid ReadingListId, int PageNumber = 0, int Count = 5)
        {
            if (Count > 10)
                Count = 10;
            if (Count < 1)
                Count = 1;

            string? userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            _ = Guid.TryParse(userId, out Guid parsedUserId);

            var query = new GetUserReadingListWithBooksQuery(parsedUserId, ReadingListId, PageNumber, Count);
            var Result = await _mediator.Send(query);

            if (Result.Item1 is null)
                return NotFound("ReadingList Not_Found");

            return Ok(new
            {
                ReadingList = Result.Item1.Adapt<GetReadingListWithBooksDto>(),
                Pages = Result.Item2
            }
            );
        }

        /// <summary>
        /// Create new ReadingList
        /// </summary>
        /// <param name="readingListDto"></param>
        /// <returns></returns>
        /// <remarks>      
        /// Sample request:
        ///
        ///     POST /ReadingList
        ///      {
        ///         name: string,
        ///         private: true
        ///      }
        /// </remarks>
        [SwaggerResponse(StatusCodes.Status201Created, "Returns the newly created ReadingList and it's route in the header:location", Type = typeof(GetReadingListDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "when Creating new ReadingList without private field", typeof(CreateReadingListErrorResponseExample))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(CreateReadingListErrorResponseExample))]
        [HasRole("patron")]
        [HttpPost(Name = "CreateReadingList")]
        public async Task<ActionResult> CreateReadingList(CreateReadingListDto readingListDto)
        {

            string? userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            _ = Guid.TryParse(userId, out Guid parsedUserId);

            var readingList = readingListDto.Adapt<ReadingList>();
            var command = new CreateReadingListCommand(parsedUserId, readingList);
            var Result = await _mediator.Send(command);
            var bookWithAuthorsDto = Result.Adapt<GetReadingListDto>();
            return CreatedAtAction(nameof(GetReadingListWithBooks), new { ReadingListId = bookWithAuthorsDto.Id }, bookWithAuthorsDto);
        }

        /// <summary>
        /// Update ReadingList by Id
        /// </summary>
        /// <param name="ReadingListId"></param>
        /// <param name="readingListDto"></param>
        /// <returns></returns>
        /// <remarks> 
        /// Sample request:
        /// 
        ///     PUT /ReadingList/D9BBC76C-AFBB-4AB0-BE01-ADCCD95C38D8 
        ///     {
        ///         id: D9BBC76C-AFBB-4AB0-BE01-ADCCD95C38D8,
        ///         name: string,
        ///         private: true
        ///     }
        /// </remarks>
        [SwaggerResponse(StatusCodes.Status200OK, "Success when ReadingList is updated")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "When ReadingList is Not Found")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "When ReadingList Id in the route does not match the one in the body or request without name", typeof(UpdateReadingListErrorResponseExample))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(UpdateReadingListErrorResponseExample))]
        [HasRole("patron")]
        [HttpPut("{ReadingListId}", Name = "UpdateReadingList")]
        public async Task<ActionResult> UpdateReadingList(Guid ReadingListId, UpdateReadingListDto readingListDto)
        {
            try
            {
                string? userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
                _ = Guid.TryParse(userId, out Guid parsedUserId);

                if (ReadingListId != readingListDto.Id)
                {
                    var errorResponse = new ErrorResponse(status: HttpStatusCode.BadRequest);
                    errorResponse.Errors?.Add(new ErrorModel() { FieldName = "Id", Message = "bad Id" });
                    return new BadRequestObjectResult(errorResponse);
                }
                var readingList = readingListDto.Adapt<ReadingList>();
                var command = new UpdateReadingListCommand(parsedUserId, readingList);
                var Result = await _mediator.Send(command);
                return Result ? Ok("ReadingList has been Updated") : StatusCode(StatusCodes.Status500InternalServerError);
            }
            catch (CustomNotFoundException e)
            {
                var errorResponse = new ErrorResponse(status: HttpStatusCode.NotFound);
                errorResponse.Errors?.Add(new ErrorModel() { FieldName = "ReadingList", Message = e.Message });
                return new NotFoundObjectResult(errorResponse);

            }
        }

        /// <summary>
        /// Delete readingList by Id
        /// </summary>
        /// <param name="ReadingListId"></param>
        /// <returns></returns>
        /// <remarks> 
        /// Sample request:
        /// 
        ///     DELETE /ReadingList/D9BBC76C-AFBB-4AB0-BE01-ADCCD95C38D8 
        /// </remarks>
        [SwaggerResponse(StatusCodes.Status200OK, "Returns Success when ReadingList is deleted")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "When ReadingList is Not Found")]
        [HasRole("patron")]
        [HttpDelete("{ReadingListId}", Name = "RemoveReadingList")]
        public async Task<ActionResult> RemoveReadingList(Guid ReadingListId)
        {
            try
            {
                string? userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
                _ = Guid.TryParse(userId, out Guid parsedUserId);

                var command = new RemoveReadingListCommand(parsedUserId, ReadingListId);
                var Result = await _mediator.Send(command);
                return Result ? Ok("Reading List has been Deleted") : StatusCode(StatusCodes.Status500InternalServerError);
            }
            catch (CustomNotFoundException e)
            {
                var errorResponse = new ErrorResponse(status: HttpStatusCode.NotFound);
                errorResponse.Errors?.Add(new ErrorModel() { FieldName = "Reading List", Message = e.Message });
                return new NotFoundObjectResult(errorResponse);

            }
        }
        /// <summary>
        /// Add book to readingList
        /// </summary>
        /// <param name="ReadingListId"></param>
        /// <param name="BookId"></param>
        /// <returns></returns>
        ///     <remarks> 
        /// Sample request:
        /// 
        ///     POST /ReadingList/D9BBC76C-AFBB-4AB0-BE01-ADCCD95C38D8/Books/BBAB7E04-92FD-42D0-BB67-050379BE5585
        /// </remarks>
        [SwaggerResponse(StatusCodes.Status200OK, "Returns Success when Book is added to the reading List")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "When ReadingList or Book Not Found", typeof(AddBookToReadingListErrorResponseExample))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(AddBookToReadingListErrorResponseExample))]
        [SwaggerResponse(StatusCodes.Status409Conflict, "when ReadingList already has The added Book")]
        [HasRole("patron")]
        [HttpPost("{ReadingListId}/Books/{BookId}", Name = "AddBookToReadingList")]
        public async Task<ActionResult> AddBookToReadingList(Guid ReadingListId, Guid BookId)
        {
            try
            {
                string? userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
                _ = Guid.TryParse(userId, out Guid parsedUserId);
                BookReadingList bookReadingList = new()
                {
                    ReadingListId = ReadingListId,
                    BookId = BookId
                };

                var command = new AddBookToUserReadingListCommand(parsedUserId, bookReadingList);
                var Result = await _mediator.Send(command);
                return Result ? Ok("Book Has Been Added To The ReadingList") : StatusCode(StatusCodes.Status500InternalServerError);
            }
            catch (CustomNotFoundException e)
            {
                var errorResponse = new ErrorResponse(status: HttpStatusCode.NotFound);
                errorResponse.Errors?.Add(new ErrorModel() { FieldName = e._field, Message = e.Message });
                return new NotFoundObjectResult(errorResponse);

            }
            catch (ReadingListContainsTheBookException e)
            {
                var errorResponse = new ErrorResponse(status: HttpStatusCode.Conflict);
                errorResponse.Errors?.Add(new ErrorModel() { FieldName = "Book", Message = e.Message });
                return new ConflictObjectResult(errorResponse);

            }
        }
        /// <summary>
        /// Remove book from readingList
        /// </summary>
        /// <param name="ReadingListId"></param>
        /// <param name="BookId"></param>
        /// <returns></returns>
        ///     <remarks> 
        /// Sample request:
        /// 
        ///     DELETE /ReadingList/D9BBC76C-AFBB-4AB0-BE01-ADCCD95C38D8/Books/BBAB7E04-92FD-42D0-BB67-050379BE5585
        /// </remarks>
        [SwaggerResponse(StatusCodes.Status200OK, "Returns Success when Book is removed from the reading List")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "When ReadingList or Book Not Found", typeof(RemoveBookFromReadingListNotFoundErrorResponseExample))]
        [SwaggerResponseExample(StatusCodes.Status404NotFound, typeof(RemoveBookFromReadingListNotFoundErrorResponseExample))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "When ReadingList Does Not Contain the Book", typeof(RemoveBookFromReadingListBadRequestErrorResponseExample))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(RemoveBookFromReadingListBadRequestErrorResponseExample))]

        [HasRole("patron")]
        [HttpDelete("{ReadingListId}/Books/{BookId}", Name = "RemoveBookFromReadingList")]
        public async Task<ActionResult> RemoveBookFromReadingList(Guid ReadingListId, Guid BookId)
        {
            try
            {
                string? userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
                _ = Guid.TryParse(userId, out Guid parsedUserId);
                BookReadingList bookReadingList = new()
                {
                    ReadingListId = ReadingListId,
                    BookId = BookId
                };

                var command = new RemoveBookFromUserReadingListCommand(parsedUserId, bookReadingList);
                var Result = await _mediator.Send(command);
                return Result ? Ok("Book Has Been Removed From The ReadingList") : StatusCode(StatusCodes.Status500InternalServerError);
            }
            catch (CustomNotFoundException e)
            {
                var errorResponse = new ErrorResponse(status: HttpStatusCode.NotFound);
                errorResponse.Errors?.Add(new ErrorModel() { FieldName = e._field, Message = e.Message });
                return new NotFoundObjectResult(errorResponse);

            }
            catch (ReadingListDoesNotContainTheBookException e)
            {
                var errorResponse = new ErrorResponse(status: HttpStatusCode.BadRequest);
                errorResponse.Errors?.Add(new ErrorModel() { FieldName = "Book", Message = e.Message });
                return new BadRequestObjectResult(errorResponse);

            }
        }
    }
}
