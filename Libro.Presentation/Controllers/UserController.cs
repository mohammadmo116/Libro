using Libro.Application.Users.Commands;
using Libro.Domain.Entities;
using Libro.Domain.Enums;
using Libro.Domain.Exceptions;
using Libro.Domain.Responses;
using Libro.Infrastructure;
using Libro.Infrastructure.Authorization;
using Libro.Presentation.Dtos.Role;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;


namespace Libro.Presentation.Controllers
{

    [ApiController]
    [Route("User")]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ApplicationDbContext _context;
        public UserController(IMediator mediator, ApplicationDbContext context)
        {
            _mediator = mediator;
            _context = context;
        }


        [HasRole("admin")]
        [HttpPost("{UserId}/AssignRole/{RoleId}", Name = "AssignRole")]
        public async Task<ActionResult<bool>> AssignRoleToUser(Guid UserId, Guid RoleId)
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
                return Result ? Ok("Role Has Been Assigned") : BadRequest();

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
        
        [HttpGet("test/{UserId}", Name = "testt")]
        public async Task<ActionResult<User>> test(Guid UserId)
        {
            var user= await _context.Users
                    .Include(a => a.Roles.Where(a=>a.Name=="patron"))
                    .Include(a=>a.BookTransactions.Where(b=>b.Status!=BookStatus.None))
                    .FirstOrDefaultAsync(u => u.Id == UserId);
            if (user.Roles.Any())
                return user;
            return Ok("d"); 
        }
    }
}
