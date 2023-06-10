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
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
                string? userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
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
                string? userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
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
        public async Task<ActionResult<(List<BookTransactionWithStatusDto>, int)>> GetBorrowingHistory(int PageNumber = 0, int Count = 5)
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
                var query = new GetBorrowingHistoryQuery(parsedUserId, PageNumber, Count);
                var Result = await _mediator.Send(query);

                return Ok(new
                {
                    ReadingList = Result.Item1.Adapt<List<BookTransactionWithStatusDto>>(),
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
                var errorResponse = new ErrorResponse(status: HttpStatusCode.BadRequest);
                errorResponse.Errors?.Add(new ErrorModel() { FieldName = "Role", Message = e.Message });
                return new BadRequestObjectResult(errorResponse);
            }
        }





    }
}
