using Libro.Application.Users.Commands;
using Libro.Application.Users.Queries;
using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using Libro.Domain.Exceptions.UserExceptions;
using Libro.Domain.Responses;
using Libro.Infrastructure.Authorization;
using Libro.Presentation.Dtos.User;
using Libro.Presentation.SwaggerExamples.Authentication;
using Libro.Presentation.SwaggerExamples.Book;
using Libro.Presentation.SwaggerExamples.Librarian;
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
    [Route("Librarian")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authorization has been denied for this request")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "When user is not Admin, or the managed user is not librarian")]
    public class LibrarianController : ControllerBase
    {
        private readonly IMediator _mediator;

        public LibrarianController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Get Librarian Profile Details by Id
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        /// <remarks> 
        /// Sample request:
        ///
        ///     GET /librarian/A63B4C43-AA43-4E90-A97E-FB8DABC3162D
        /// </remarks>
        [SwaggerResponse(StatusCodes.Status200OK, "Returns Librarian's Profile",typeof(UserDto))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "When Librarian is Not Found")]
        [HasRole("admin")]
        [ToRole("librarian")]
        [HttpGet("{UserId}", Name = "GetLibrarianById")]
        public async Task<ActionResult<UserDto>> GetLibrarianById(Guid UserId)
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
        /// <summary>
        /// Create New Librarian account
        /// </summary>
        /// <param name="createUserDto"></param>
        /// <returns></returns>
        /// <remarks> 
        /// Sample request:
        ///
        ///     POST /librarian
        ///     {
        ///         "userName": "string",
        ///         "email": "user@example.com",
        ///         "phoneNumber": "string",
        ///         "password": "string"
        ///      }
        ///    Sample request2:
        ///
        ///     POST /librarian
        ///     {
        ///          userName: LibrarianNameTest,
        ///          email: LibrarianEmail@gmail.com,
        ///          password: testPassword
        ///     }    
        /// </remarks>
        [SwaggerResponse(StatusCodes.Status201Created, "Returns the newly created Librarian and it's route in the header:location", Type = typeof(UserDtoWithId))]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "When user is not Admin")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "When Role librarian is not Found")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "When user info (Email,UserName,Phone) Already Exists", Type = typeof(RegisterLibraraianErrorResponseExample))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(RegisterLibraraianErrorResponseExample))]
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
                return CreatedAtAction(nameof(GetLibrarianById), new { UserId = ResultUserDto.Id }, ResultUserDto);

            }

            catch (UserExistsException e)
            {
                var errorResponse = new ErrorResponse(status: HttpStatusCode.BadRequest);
                errorResponse.Errors.Add(new ErrorModel() { FieldName = e._field, Message = e.Message });
                return new BadRequestObjectResult(errorResponse);

            }
            catch (CustomNotFoundException e)
            {
                var errorResponse = new ErrorResponse(status: HttpStatusCode.NotFound);
                errorResponse.Errors.Add(new ErrorModel() { FieldName = "Role", Message = e.Message });
                return new NotFoundObjectResult(errorResponse);

            }
        }
        /// <summary>
        /// update Librarian Profile
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="userDto"></param>
        /// <returns></returns>
        /// <remarks> 
        /// Sample request:
        /// 
        ///     PUT /librarian/A63B4C43-AA43-4E90-A97E-FB8DABC3162D 
        ///     {
        ///         id: A63B4C43-AA43-4E90-A97E-FB8DABC3162D,
        ///         userName: librarianUserName,
        ///         email: test@test.com,
        ///         phoneNumber: 059248215
        ///     }
        /// </remarks>
        [SwaggerResponse(StatusCodes.Status200OK, "Success when Librarian is updated")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "When Librarian is Not Found")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "When the new Librarian info already exists or the LibrarianId in the route does not match the one in the body", typeof(UpdateLibraraianErrorResponseExample))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(UpdateLibraraianErrorResponseExample))]

        [HasRole("admin")]
        [ToRole("librarian")]
        [HttpPut("{UserId}", Name = "UpdateLibrarianUser")]
        public async Task<ActionResult> UpdateLibrarianUser(Guid UserId, UpdateUserDto userDto)
        {
            try
            {
                if (UserId != userDto.Id)
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
        /// Delete Librarian account
        /// </summary>
        /// <param name="UserId"></param>
        /// <returns></returns>
        /// <remarks> 
        /// Sample request:
        /// 
        ///     DELETE /librarian/A63B4C43-AA43-4E90-A97E-FB8DABC3162D 
        /// </remarks>
        [SwaggerResponse(StatusCodes.Status200OK, "Success when Librarian is Deleted")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "When Librarian is Not Found")]
        [HasRole("admin")]
        [ToRole("librarian")]
        [HttpDelete("{UserId}", Name = "RemoveLibrarianUser")]
        public async Task<ActionResult> RemoveLibrarianUser(Guid UserId)
        {
            try
            {

                var command = new RemoveUserCommand(UserId);
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
