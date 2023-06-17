using Libro.Application.Users.Commands;
using Libro.Application.Users.Queries;
using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using Libro.Domain.Exceptions.UserExceptions;
using Libro.Domain.Responses;
using Libro.Infrastructure.Authorization;
using Libro.Presentation.Dtos.Book;
using Libro.Presentation.Dtos.BookTransaction;
using Libro.Presentation.Dtos.Notifications;
using Libro.Presentation.Dtos.User;
using Libro.Presentation.SwaggerExamples.Book;
using Libro.Presentation.SwaggerExamples.Librarian;
using Libro.Presentation.SwaggerExamples.Patron;
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
    [Route("Patron")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authorization has been denied for this request")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "When user is not Admin or Librarian, or the managed user is not Patron")]
    public class PatronController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PatronController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Get patron user by admin or librarian by userId
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        /// <remarks> 
        /// Sample request:
        ///
        ///     GET Patron/CC0D402A-D4AA-40CE-B675-B4F67EC4B650
        /// </remarks>
        [SwaggerResponse(StatusCodes.Status200OK, "Returns Patron's Profile", Type = typeof(UserDto))]
        [HasRole("admin,librarian")]
        [ToRole("patron")]
        [HttpGet("{UserId}", Name = "GetPatronUser")]
        public async Task<ActionResult<UserDto>> GetPatronUser(Guid UserId)
        {
            try
            {
                var query = new GetUserQuery(UserId);
                var Result = await _mediator.Send(query);
                return Ok(Result.Adapt<UserDto>());

            }
            catch (CustomNotFoundException e)
            {
                var errorResponse = new ErrorResponse(status: HttpStatusCode.NotFound);
                errorResponse.Errors?.Add(new ErrorModel() { FieldName = "User", Message = e.Message });
                return new BadRequestObjectResult(errorResponse);

            }

        }
        /// <summary>
        ///  Updates Patron's Profile
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="userDto"></param>
        /// <returns></returns>
        /// <remarks> 
        /// Sample request:
        /// 
        ///     PUT /Patron/A63B4C43-AA43-4E90-A97E-FB8DABC3162D 
        ///     {
        ///         id: A63B4C43-AA43-4E90-A97E-FB8DABC3162D,
        ///         userName: PatronUserName,
        ///         email: test@test.com,
        ///         phoneNumber: 0592482135
        ///     }
        /// </remarks>
        [SwaggerResponse(StatusCodes.Status200OK, "Success when Patron is updated")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "When the new Patron info already exists or the PatronId in the route does not match the one in the body", typeof(UpdatePatronErrorResponseExample))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(UpdatePatronErrorResponseExample))]
        [HasRole("admin,librarian")]
        [ToRole("patron")]
        [HttpPut("{UserId}", Name = "UpdatePatronUser")]
        public async Task<ActionResult> UpdatePatronUser(Guid UserId, UpdateUserDto userDto)
        {
            try
            {
                if (UserId != userDto.Id)
                {
                    var errorResponse = new ErrorResponse(status: HttpStatusCode.BadRequest);
                    errorResponse.Errors?.Add(new ErrorModel() { FieldName = "Id", Message = "bad Id" });
                    return new BadRequestObjectResult(errorResponse);
                }
                var user = userDto.Adapt<User>();
                var query = new UpdateUserCommand(user);
                var Result = await _mediator.Send(query);

                return Result ? Ok("Profile has heen Updated") : StatusCode(StatusCodes.Status500InternalServerError);

            }
            catch (CustomNotFoundException e)
            {
                var errorResponse = new ErrorResponse(status: HttpStatusCode.NotFound);
                errorResponse.Errors?.Add(new ErrorModel() { FieldName = "User", Message = e.Message });
                return new BadRequestObjectResult(errorResponse);

            }
            catch (UserExistsException e)
            {
                var errorResponse = new ErrorResponse(status: HttpStatusCode.BadRequest);
                errorResponse.Errors.Add(new ErrorModel() { FieldName = e._field, Message = e.Message });
                return new BadRequestObjectResult(errorResponse);

            }
        }
        /// <summary>
        /// Get patron's Borrowing History
        /// </summary>
        /// <param name="UserId"></param>
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
        ///     GET /Patron/A63B4C43-AA43-4E90-A97E-FB8DABC3162D/Borrowing-History?PageNumber=0&amp;Count=5
        /// </remarks>
       
        [SwaggerResponse(StatusCodes.Status200OK, "List of Transactions with pagination",typeof(GetPatronBorrowingHistoryOkResponseExample))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(GetPatronBorrowingHistoryOkResponseExample))]
        [HasRole("admin,librarian")]
        [ToRole("patron")]
        [HttpGet("{UserId}/Borrowing-History", Name = "GetPatronBorrwingHistory")]
        public async Task<ActionResult<(List<BookTransactionWithStatusDto>,int)>> GetPatronBorrowingHistory(Guid UserId, int PageNumber = 0, int Count = 5)
        {
            try
            {
                if (Count > 10)
                    Count = 10;
                if (Count < 1)
                    Count = 1;

                var query = new GetBorrowingHistoryQuery(UserId, PageNumber, Count);
                var Result = await _mediator.Send(query);

                return Ok(new
                {
                    Transactions = Result.Item1.Adapt<List<BookTransactionWithStatusDto>>(),
                    Pages = Result.Item2
                });


            }
            catch (CustomNotFoundException e)
            {
                var errorResponse = new ErrorResponse(status: HttpStatusCode.NotFound);
                errorResponse.Errors?.Add(new ErrorModel() { FieldName = "User", Message = e.Message });
                return new BadRequestObjectResult(errorResponse);

            }

        }
        /// <summary>
        /// Get Recommended Books for a patron based in borrowing History
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
        ///     GET /Patron/RecommendedBooks?PageNumber=0&amp;Count=5
        /// </remarks>
        [SwaggerResponse(StatusCodes.Status200OK, "List of Recommended Books with pagination", typeof(GetPatronRecommendedBooksPaginationOkResultExample))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(GetPatronRecommendedBooksPaginationOkResultExample))]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "When user is not Patron")]
        [HasRole("patron")]
        [HttpGet("RecommendedBooks", Name = "GetRecommendedBooks")]
        public async Task<ActionResult<(List<BookWithAuthorsDto>, int)>> GetPatronRecommendedBooks(int PageNumber = 0, int Count = 5)
        {
                if (Count > 10)
                    Count = 10;
                if (Count < 1)
                    Count = 1;
                string? userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
                _=Guid.TryParse(userId, out Guid parsedUserId);
              
                var query = new GetRecommendedBooksQuery(parsedUserId, PageNumber, Count);
                var Result = await _mediator.Send(query);

                return Ok(new
                {
                    RecommendedBooks = Result.Item1.Adapt<List<BookWithAuthorsDto>>(),
                    Pages = Result.Item2
                });       

        }

    }
}
