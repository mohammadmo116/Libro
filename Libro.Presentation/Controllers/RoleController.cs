using Libro.Application.Roles.Commands;
using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using Libro.Domain.Responses;
using Libro.Infrastructure.Authorization;
using Libro.Presentation.Dtos.Author;
using Libro.Presentation.Dtos.Role;
using Libro.Presentation.Dtos.User;
using Libro.Presentation.SwaggerExamples.Author;
using Libro.Presentation.SwaggerExamples.Role;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using System.Net;

namespace Libro.Presentation.Controllers
{
    [ApiController]
    [Route("Role")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authorization has been denied for this request")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "When user is not Admin")]
    public class RoleController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RoleController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Create new role
        /// </summary>
        /// <param name="createRoleDto"></param>
        /// <returns></returns>
        ///     <remarks> 
        /// Sample request:
        /// 
        ///     POST /Role
        ///     {
        ///         name = roleName
        ///     }
        /// </remarks>
        [SwaggerResponse(StatusCodes.Status200OK, "Success and returns the newly created role",typeof(RoleDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "when Role Name Already Exists", typeof(CreateRoleErrorResponseExample))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(CreateRoleErrorResponseExample))]
        [HasRole("admin")]
        [HttpPost(Name = "AddRole")]
        public async Task<ActionResult<UserDto>> CreateRole(CreateRoleDto createRoleDto)
        {
            try
            {
                var role = createRoleDto.Adapt<Role>();
                var query = new CreateRoleCommand(role);
                var Result = await _mediator.Send(query);
                return Ok(Result.Adapt<RoleDto>());
            }
            catch (RoleExistsException e)
            {
                var errorResponse = new ErrorResponse(status: HttpStatusCode.BadRequest);
                errorResponse.Errors?.Add(new ErrorModel() { FieldName = "Name", Message = e.Message });
                return new BadRequestObjectResult(errorResponse);

            }

        }

    }
}
