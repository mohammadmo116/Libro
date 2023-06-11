using Libro.Application.Users.Commands;
using Libro.Application.Users.Queries;
using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using Libro.Domain.Exceptions.UserExceptions;
using Libro.Domain.Responses;
using Libro.Infrastructure.Authorization;
using Libro.Presentation.Dtos.Book;
using Libro.Presentation.Dtos.BookTransaction;
using Libro.Presentation.Dtos.User;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace Libro.Presentation.Controllers
{
    [ApiController]
    [Route("Patron")]
    public class PatronController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PatronController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HasRole("admin,librarian")]
        [ToRole("patron")]
        [HttpGet("{UserId}", Name = "GetPatronUser")]
        public async Task<ActionResult<UserDtoWithId>> GetPatronUser(Guid UserId)
        {
            try
            {
                var query = new GetUserQuery(UserId);
                var Result = await _mediator.Send(query);
                return Ok(Result.Adapt<UserDtoWithId>());

            }
            catch (CustomNotFoundException e)
            {
                var errorResponse = new ErrorResponse(status: HttpStatusCode.NotFound);
                errorResponse.Errors?.Add(new ErrorModel() { FieldName = "User", Message = e.Message });
                return new BadRequestObjectResult(errorResponse);

            }

        }

        [HasRole("admin,librarian")]
        [ToRole("patron")]
        [HttpPut("{UserId}", Name = "UpdatePatronUser")]
        public async Task<ActionResult> UpdatePatronUser(Guid UserId, UpdateUserDto userDto)
        {
            try
            {
                if (UserId != userDto.Id)
                {
                    return BadRequest();
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
        [HasRole("admin,librarian")]
        [ToRole("patron")]
        [HttpGet("{UserId}/BorrowingHistory", Name = "GetPatronBorrwingHistory")]
        public async Task<ActionResult<List<BookTransactionWithStatusDto>>> GetPatronBorrowingHistory(Guid UserId, int PageNumber = 0, int Count = 5)
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
        [HasRole("patron")]
        [HttpGet("RecommendedBooks", Name = "GetRecommendedBooks")]
        public async Task<ActionResult<(List<BookWithAuthorsDto>, int)>> GetPatronRecommendedBooks(int PageNumber = 0, int Count = 5)
        {
            try
            {

                if (Count > 10)
                    Count = 10;
                if (Count < 1)
                    Count = 1;
                string? userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
                if (!Guid.TryParse(userId, out Guid parsedUserId))
                {
                    return BadRequest("Bad user Id");
                }
                var query = new GetRecommendedBooksQuery(parsedUserId, PageNumber, Count);
                var Result = await _mediator.Send(query);

                return Ok(new
                {
                    RecommendedBooks = Result.Item1.Adapt<List<BookWithAuthorsDto>>(),
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

    }
}
