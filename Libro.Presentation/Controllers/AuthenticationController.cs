using Libro.Application.Users.Commands;
using Libro.Application.Users.Queries;
using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using Libro.Domain.Exceptions.UserExceptions;
using Libro.Domain.Responses;
using Libro.Presentation.Dtos.Book;
using Libro.Presentation.Dtos.User;
using Libro.Presentation.SwaggerExamples.Authentication;
using Libro.Presentation.SwaggerExamples.Librarian;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;
using System.Net;
using System.Security.Authentication;

namespace Libro.Presentation.Controllers
{
    [ApiController]
    [Route("Authentication")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AuthenticationController(IMediator mediator)
        {
            _mediator = mediator;
        }
        /// <summary>
        /// Rgister New User
        /// </summary>
        /// <param name="createUserDto"></param>
        /// <returns></returns>
        /// <remarks> 
        /// Sample request:
        ///
        ///     POST /Authentication/Rgister
        ///     {
        ///          userName: userNameTest,
        ///          email: userEmail@gmail.com,
        ///          phoneNumber: 0593497415,
        ///          password: testPassword
        ///     }
        ///    Sample request2:
        ///
        ///     POST /Rgister
        ///     {
        ///          userName: userNameTest,
        ///          email: userEmail@gmail.com,
        ///          password: testPassword
        ///     }
        /// </remarks>
        [SwaggerResponse(StatusCodes.Status200OK, description: "Return the User's info when Rgister sucessfully", Type = typeof(UserDtoWithId))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "When user info (Email,UserName,Phone) Already Exists", Type = typeof(RegisterErrorResponseExample))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "When Role patron is not Found")]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(RegisterErrorResponseExample))]
        [HttpPost("Rgister", Name = "Rgister")]
        public async Task<ActionResult<UserDtoWithId>> RgisterUser(CreateUserDto createUserDto)
        {
            try
            {
                var user = createUserDto.Adapt<User>();
                var query = new CreateUserByRoleCommand(user,"patron");
                var Result = await _mediator.Send(query);
                return Ok(Result.Adapt<UserDtoWithId>());
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
        /// Login to account
        /// </summary>
        /// <param name="loginUserDto"></param>
        /// <returns></returns>
        /// <remarks> 
        /// Sample request:
        ///
        ///     POST /Authentication/Login
        ///     { 
        ///          email: user@example.com,
        ///          password: string
        ///     }
        /// </remarks>
        [SwaggerResponse(StatusCodes.Status200OK, description: "Return the User's Token when login sucessfully", Type = typeof(string))]
        [SwaggerResponse(StatusCodes.Status401Unauthorized, "When user's credentials are Invalid")]
        [HttpPost("Login", Name = "Login")]
        public async Task<ActionResult<string>> Authenticate(LoginUserDto loginUserDto)
        {
            try
            {
                var query = new LoginUserQuery(loginUserDto.Email, loginUserDto.Password);
                var Result = await _mediator.Send(query);
                return Ok(Result);
            }
            catch (InvalidCredentialException e)
            {
                return Unauthorized(e.Message);
            }


        }


    }
}
