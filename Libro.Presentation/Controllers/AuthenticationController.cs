using Libro.Application.Users.Commands;
using Libro.Application.WeatherForecasts.Queries;
using Libro.Domain.Entities;
using Libro.Presentation.Dtos.User;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public async Task<ActionResult<GetUserDto>> RgisterUser(CreateUserDto createUserDto)
        {
            var user=createUserDto.Adapt<User>();
            var query = new CreateUserCommand(user);
            var Result = await _mediator.Send(query);
            return Ok(Result.Adapt<GetUserDto>());
        }
    }
}
