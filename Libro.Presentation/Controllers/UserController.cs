using Libro.Application.Books.Commands;
using Libro.Application.BookTransactions.Commands;
using Libro.Application.Users.Commands;
using Libro.Application.Users.Queries;
using Libro.Domain.Entities;
using Libro.Domain.Enums;
using Libro.Domain.Exceptions;
using Libro.Domain.Responses;
using Libro.Infrastructure;
using Libro.Infrastructure.Authorization;
using Libro.Presentation.Dtos.Book;
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
        [HasRole("admin")]
        [HttpGet("{UserId}",Name = "GetUserById")]
        public async Task<ActionResult<UserDto>> GetUserById(Guid UserId)
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


        [HasRole("admin")]
        [HttpPost("Librarian", Name = "CreateLibrarianUser")]
        public async Task<ActionResult> CreateLibrarianUser(CreateUserDto createUserDto)
        {
            try
            {
                var user = createUserDto.Adapt<User>();
                var query = new CreateUserByRoleCommand(user, "librarian");
                var Result = await _mediator.Send(query);
                var ResultUserDto = Result.Adapt<UserDtoWithId>();
                return CreatedAtAction(nameof(GetUserById), new { UserId = ResultUserDto.Id }, ResultUserDto);

            }

            catch (UserExistsException e)
            {
                var errorResponse = new ErrorResponse(status: HttpStatusCode.BadRequest);
                errorResponse.Errors.Add(new ErrorModel() { FieldName = e._field, Message = e.Message });
                return new BadRequestObjectResult(errorResponse);

            }
        }
        [HasRole("admin")]
        [HttpPut("Librarian/{LibrarianId}", Name = "UpdateLibrarianUser")]
        public async Task<ActionResult> UpdateLibrarianUser(Guid LibrarianId, UpdateUserDto userDto)
        {
            try
            {
                if (LibrarianId != userDto.Id)
                {
                    return BadRequest();
                }

                var user = userDto.Adapt<User>();
                var query = new UpdateUserByRoleCommand(user, "Librarian");
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
        [HasRole("admin")]
        [HttpDelete("librarian/{LibrarianId}", Name = "RemoveLibrarianUser")]
        public async Task<ActionResult> RemoveLibrarianUser(Guid LibrarianId)
        {
            try
            {

                var command = new RemoveUserByRoleCommand(LibrarianId,"librarian");
                var Result = await _mediator.Send(command);
                return Result ? Ok("Librarian User has been Deleted") : StatusCode(StatusCodes.Status500InternalServerError);
            }
            catch (CustomNotFoundException e)
            {
                var errorResponse = new ErrorResponse(status: HttpStatusCode.NotFound);
                errorResponse.Errors?.Add(new ErrorModel() { FieldName = "User", Message = e.Message });
                return new BadRequestObjectResult(errorResponse);

            }
        }
        [HasRole("admin,librarian")]
        [HttpGet("Patron/{PatronId}", Name = "GetPatronUser")]
        public async Task<ActionResult<UserDtoWithId>> GetPatronUser(Guid PatronId)
        {
            try
            {
                var query = new GetPatronUserQuery(PatronId);
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
        [HttpPut("Patron/{PatronId}", Name = "UpdatePatronUser")]
        public async Task<ActionResult> UpdatePatronUser(Guid PatronId, UpdateUserDto userDto)
        {
            try
            {
                if (PatronId != userDto.Id)
                { 
                    return BadRequest();
                }

                var user = userDto.Adapt<User>();
                var query = new UpdateUserByRoleCommand(user,"patron");
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
        [HttpGet("Patron/{PatronId}/BorrowingHistory", Name = "GetPatronBorrwingHistory")]
        public async Task<ActionResult<List<BookTransactionWithStatusDto>>> GetPatronBorrowingHistory(Guid PatronId, int PageNumber = 0, int Count = 5)
        {
            try
            { 
                var query = new GetPatronBorrowingHistoryQuery(PatronId, PageNumber, Count);
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
