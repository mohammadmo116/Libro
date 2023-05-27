using Libro.Application.Roles.Commands;
using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using Libro.Domain.Responses;
using Libro.Infrastructure.Authorization;
using Libro.Presentation.Dtos.Role;
using Libro.Presentation.Dtos.User;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Libro.Presentation.Controllers
{
    [ApiController]
    [Route("Role")]
    public class RoleController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RoleController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HasRole("admin")]
        [HttpPost("AddRole", Name = "AddRole")]
        public async Task<ActionResult<UserDto>> CreateRole(CreateRoleDto createRoleDto)
        {
            try
            {
                var role = createRoleDto.Adapt<Role>();
                var query = new CreateRoleCommand(role);
                var Result = await _mediator.Send(query);
                return Ok(Result.Adapt<RoleDto>());
            }
            catch (RoleExistsException e) {
                var errorResponse = new ErrorResponse(status: HttpStatusCode.BadRequest);
                errorResponse.Errors?.Add(new ErrorModel() { FieldName ="Role", Message = e.Message }) ;
                return new BadRequestObjectResult(errorResponse);

            }

        }
      
    }
}
