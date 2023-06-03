using Libro.Application.BookTransactions.Commands;
using Libro.Application.Users.Commands;
using Libro.Application.Users.Queries;
using Libro.Domain.Entities;
using Libro.Domain.Enums;
using Libro.Domain.Exceptions;
using Libro.Domain.Responses;
using Libro.Infrastructure;
using Libro.Infrastructure.Authorization;
using Libro.Presentation.Dtos.BookTransaction;
using Libro.Presentation.Dtos.Role;
using Libro.Presentation.Dtos.User;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security.Claims;

namespace Libro.Presentation.Controllers
{

    [ApiController]
    [Route("User")]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;
        public UserController(IMediator mediator)
        {
            _mediator = mediator;

        }
        [Authorize()]
        [HttpGet(Name = "GetUser")]
        public async Task<ActionResult<UserDto>> GetUser()
        {
            try
            {
                string? userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid)?.Value;
                if (!Guid.TryParse(userId, out Guid parsedUserId))
                {
                    return BadRequest("Bad user Id");
                }
                var query = new GetUserQuery(parsedUserId);
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

        [Authorize()]
        [HttpPut(Name = "UpdateUser")]
        public async Task<ActionResult> UpdateUser(UpdateUserDto userDto)
        {
            try
            {
                string? userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid)?.Value;
                if (!Guid.TryParse(userId, out Guid parsedUserId))
                {
                    return BadRequest("Bad user Id");
                }
                if (parsedUserId != userDto.Id)
                {
                    return BadRequest("Wrong Id");
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
        [Authorize()]
        [HttpGet("BorrowingHistory", Name = "GetBorrowingHistory")]
        public async Task<ActionResult<List<BookTransactionWithStatusDto>>> GetBorrowingHistory(int PageNumber = 0, int Count = 5)
        {
            try
            {
                string? userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid)?.Value;
                if (!Guid.TryParse(userId, out Guid parsedUserId))
                {
                    return BadRequest("Bad user Id");
                }
                var query = new GetBorrowingHistoryQuery(parsedUserId, PageNumber, Count);
                var Result = await _mediator.Send(query);
                return Ok(Result.Adapt<List<BookTransactionWithStatusDto>>());

            }
            catch (CustomNotFoundException e)
            {
                var errorResponse = new ErrorResponse(status: HttpStatusCode.NotFound);
                errorResponse.Errors?.Add(new ErrorModel() { FieldName = "User", Message = e.Message });
                return new BadRequestObjectResult(errorResponse);

            }

        }
        [HasRole("admin,librarian")]
        [HttpGet("{UserId}", Name = "GetPatronUser")]
        public async Task<ActionResult<UserDtoWithId>> GetPatronUser(Guid UserId)
        {
            try
            {
                var query = new GetPatronUserQuery(UserId);
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
                var query = new UpdatePatronUserCommand(user);
                var Result = await _mediator.Send(query);
                return Result? Ok("Profile has heen Updated") : StatusCode(StatusCodes.Status500InternalServerError);

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
        [HttpGet("{UserId}/BorrowingHistory", Name = "GetPatronBorrwingHistory")]
        public async Task<ActionResult<List<BookTransactionWithStatusDto>>> GetPatronBorrowingHistory(Guid UserId, int PageNumber = 0, int Count = 5)
        {
            try
            { 
                var query = new GetPatronBorrowingHistoryQuery(UserId, PageNumber, Count);
                var Result = await _mediator.Send(query);
                return Ok(Result.Adapt<List<BookTransactionWithStatusDto>>());

            }
            catch (CustomNotFoundException e)
            {
                var errorResponse = new ErrorResponse(status: HttpStatusCode.NotFound);
                errorResponse.Errors?.Add(new ErrorModel() { FieldName = "User", Message = e.Message });
                return new BadRequestObjectResult(errorResponse);

            }

        }
        [HasRole("admin")]
        [HttpPost("{UserId}/Role/{RoleId}", Name = "AssignRole")]
        public async Task<ActionResult> AssignRoleToUser(Guid UserId, Guid RoleId)
        {

            AddRoleToUserDto addRoleToUserDto = new()
            {
                RoleId = RoleId,
                UserId = UserId,
            };
            try
            {
                var userRole = addRoleToUserDto.Adapt<UserRole>();
                var query = new AddRoleToUserCommand(userRole);
                var Result = await _mediator.Send(query);
                return Result ? Ok("Role Has Been Assigned") : StatusCode(StatusCodes.Status500InternalServerError);

            }
            catch (UserOrRoleNotFoundException e)
            {
                var errorResponse = new ErrorResponse(status: HttpStatusCode.NotFound);
                errorResponse.Errors?.Add(new ErrorModel() { FieldName = "UserOrRole", Message = e.Message });
                return new BadRequestObjectResult(errorResponse);
            }
            catch (UserHasTheAssignedRoleException e)
            {
                var errorResponse = new ErrorResponse(status: HttpStatusCode.BadRequest);
                errorResponse.Errors?.Add(new ErrorModel() { FieldName = "Role", Message = e.Message });
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

     
       
    }
}
