using Libro.Application.Users.Commands;
using Libro.Application.Users.Queries;
using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using Libro.Domain.Exceptions.UserExceptions;
using Libro.Domain.Responses;
using Libro.Infrastructure.Authorization;
using Libro.Presentation.Dtos.User;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Presentation.Controllers
{
    [ApiController]
    [Route("Librarian")]
    public class LibrarianController : ControllerBase
    {
        private readonly IMediator _mediator;

        public LibrarianController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HasRole("admin")]
        [HttpGet("{Librarian}", Name = "GetLibrarianById")]
        public async Task<ActionResult<UserDto>> GetUserById(Guid LibrarianId)
        {
            try
            {

                var query = new GetUserQuery(LibrarianId);
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
        [HttpPost("", Name = "CreateLibrarianUser")]
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
        [HttpPut("{LibrarianId}", Name = "UpdateLibrarianUser")]
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
        [HttpDelete("{LibrarianId}", Name = "RemoveLibrarianUser")]
        public async Task<ActionResult> RemoveLibrarianUser(Guid LibrarianId)
        {
            try
            {

                var command = new RemoveUserByRoleCommand(LibrarianId, "librarian");
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
     
    }
}
