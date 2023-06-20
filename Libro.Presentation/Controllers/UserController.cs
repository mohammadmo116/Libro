using Libro.Application.Users.Commands;
using Libro.Application.Users.Queries;
using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using Libro.Domain.Exceptions.UserExceptions;
using Libro.Domain.Responses;
using Libro.Infrastructure.Authorization;
using Libro.Presentation.Dtos.BookTransaction;
using Libro.Presentation.Dtos.Role;
using Libro.Presentation.Dtos.User;
using Libro.Presentation.SwaggerExamples.Book;
using Libro.Presentation.SwaggerExamples.BookTransaction;
using Libro.Presentation.SwaggerExamples.Librarian;
using Libro.Presentation.SwaggerExamples.User;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using System.Net;
using System.Security.Claims;

namespace Libro.Presentation.Controllers
{

    [ApiController]
    [Route("User")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authorization has been denied for this request")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "When user is not Admin, Librarian or Patron")]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;
        public UserController(IMediator mediator)
        {
            _mediator = mediator;

        }
        /// <summary>
        /// Rerurns profile
        /// </summary>
        /// <returns></returns>
        ///     <remarks> 
        /// Sample request:
        /// 
        ///     GET /User
        /// </remarks>      
        [HasRole("librarian,admin,patron")]
        [HttpGet(Name = "GetUser")]
        [SwaggerResponse(StatusCodes.Status200OK, "Returns User's Profile",typeof(UserDto))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "When User is Not Found")]
        public async Task<ActionResult<UserDto>> GetUser()
        {
            try
            {
                string? userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
                _ = Guid.TryParse(userId, out Guid parsedUserId);
                
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

        /// <summary>
        /// Update profile
        /// </summary>
        /// <param name="userDto"></param>
        /// <returns></returns>
        ///     <remarks> 
        /// Sample request:
        /// 
        ///     PUT /User
        ///     {
        ///       id: 3fa85f64-5717-4562-b3fc-2c963f66afa6,
        ///       userName: string,
        ///       email: user@example.com,
        ///       phoneNumber: string
        ///     }
        /// </remarks>  
        [SwaggerResponse(StatusCodes.Status400BadRequest, "When the new User info already exists or the User Id in the route does not match the one in the body", typeof(UpdateUserErrorResponseExample))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(UpdateUserErrorResponseExample))]
        [HasRole("librarian,admin,patron")]
        [HttpPut(Name = "UpdateUser")]
        public async Task<ActionResult> UpdateUser(UpdateUserDto userDto)
        {
            try
            {
                string? userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
                _ = Guid.TryParse(userId, out Guid parsedUserId);
                
                if (parsedUserId != userDto.Id)
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
        /// Get Borrowing History 
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
        ///     GET /User/Borrowing-History?PageNumber=0&amp;Count=5
        /// </remarks>
        [SwaggerResponse(StatusCodes.Status200OK, "List of Borrowing History (Transactions) with pagination", typeof(GetTransactionsPaginationOkResultExample))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(GetTransactionsPaginationOkResultExample))]
        [HasRole("librarian,admin,patron")]
        [HttpGet("Borrowing-History", Name = "GetBorrowingHistory")]
        public async Task<ActionResult<(List<BookTransactionWithStatusAndIdDto>, int)>> GetBorrowingHistory(int PageNumber = 0, int Count = 5)
        {
            try
            {

                if (Count > 10)
                    Count = 10;
                if (Count < 1)
                    Count = 1;
                string? userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
                _ = Guid.TryParse(userId, out Guid parsedUserId);
                
                var query = new GetBorrowingHistoryQuery(parsedUserId, PageNumber, Count);
                var Result = await _mediator.Send(query);

                return Ok(new
                {
                    Transactions = Result.Item1.Adapt<List<BookTransactionWithStatusAndIdDto>>(),
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
        /// Assign Role To User by admin
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="RoleId"></param>
        /// <returns></returns>
        ///     <remarks> 
        /// Sample request:
        /// 
        ///     POST /User/7841A4A4-4E6A-4D5A-AADD-AA162EA1CABF/Role/BBAB7E04-92FD-42D0-BB67-050379BE5784
        /// </remarks> 
        [SwaggerResponse(StatusCodes.Status200OK, "when add role to user successfully ")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "When User Or Role Not Found")]
        [SwaggerResponse(StatusCodes.Status409Conflict, "when User already has The Assigned Role")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "When user is not Admin")]
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
                return new NotFoundObjectResult(errorResponse);
            }
            catch (UserHasTheAssignedRoleException e)
            {
                var errorResponse = new ErrorResponse(status: HttpStatusCode.Conflict);
                errorResponse.Errors?.Add(new ErrorModel() { FieldName = "Role", Message = e.Message });
                return new ConflictObjectResult(errorResponse);
            }
        }





    }
}
