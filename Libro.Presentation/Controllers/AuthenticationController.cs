using Libro.Application.Users.Commands;
using Libro.Application.Users.Queries;
using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using Libro.Domain.Responses;
using Libro.Presentation.Dtos.User;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Authentication;

namespace Libro.Presentation.Controllers
{
    [ApiController]
    [Route("Authentication")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AuthenticationController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost("Rgister",Name = "Rgister")]
        public async Task<ActionResult<UserDtoWithId>> RgisterUser(CreateUserDto createUserDto)
        {
            try
            {
                var user = createUserDto.Adapt<User>();  
                var query = new CreateUserCommand(user);
                var Result = await _mediator.Send(query);
                return Ok(Result.Adapt<UserDtoWithId>());
            }
           
            catch (UserExistsException e)
            {
                var errorResponse = new ErrorResponse(status: HttpStatusCode.BadRequest);
                errorResponse.Errors.Add(new ErrorModel() { FieldName= e._field,Message=e.Message});
                return new BadRequestObjectResult(errorResponse);
        
            }
            

        }
       
        [HttpPost("Login", Name = "Login")]
        public async Task<ActionResult<string>> Authenticate(LoginUserDto loginUserDto)
        {   
            try
            {
                var query = new LoginUserQuery(loginUserDto.Email, loginUserDto.Password);
                var Result = await _mediator.Send(query);
                return Ok(Result);
            }
            catch(InvalidCredentialException e) { 
                return Unauthorized(e.Message);
            }
            
     
        }

      
    }
}
