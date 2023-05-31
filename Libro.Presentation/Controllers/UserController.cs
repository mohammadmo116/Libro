using Libro.Application.Users.Commands;
using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using Libro.Domain.Responses;
using Libro.Infrastructure;
using Libro.Infrastructure.Authorization;
using Libro.Presentation.Dtos.Role;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;


namespace Libro.Presentation.Controllers
{

    [ApiController]
    [Route("User")]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator, ApplicationDbContext context)
        {
            _mediator = mediator;

        }


        [HasRole("admin")]
        [HttpPost("{UserId}/AssignRole/{RoleId}", Name = "AssignRole")]
        public async Task<ActionResult<bool>> AssignRoleToUser(Guid UserId, Guid RoleId)
        {
            
            AddRoleToUserDto addRoleToUserDto = new() { 
            RoleId = RoleId,
            UserId = UserId,
            };
            try
            {
                var userRole = addRoleToUserDto.Adapt<UserRole>();
                var query = new AddRoleToUserCommand(userRole);
                var Result = await _mediator.Send(query);
                return Result ?  Ok("Role Has Been Assigned") :  BadRequest();

            }
            catch (UserOrRoleNotFoundException e)
            {
                return NotFound(e.Message);
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
