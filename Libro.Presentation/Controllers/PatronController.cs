using Libro.Application.Users.Commands;
using Libro.Application.Users.Queries;
using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using Libro.Domain.Exceptions.UserExceptions;
using Libro.Domain.Responses;
using Libro.Infrastructure.Authorization;
using Libro.Presentation.Dtos.BookTransaction;
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
    [Route("Patron")]
    public class PatronController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PatronController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HasRole("admin,librarian")]
        [HttpGet("{PatronId}", Name = "GetPatronUser")]
        public async Task<ActionResult<UserDtoWithId>> GetPatronUser(Guid PatronId)
        {
            try
            {
                var query = new GetPatronUserQuery(PatronId);
                var Result = await _mediator.Send(query);
                return Ok(Result.Adapt<UserDtoWithId>());

            }
            catch (CustomNotFoundException e)
            {
                var errorResponse = new ErrorResponse(status: HttpStatusCode.NotFound);
                errorResponse.Errors?.Add(new ErrorModel() { FieldName = "User", Message = e.Message });
                return new BadRequestObjectResult(errorResponse);

            }

        }

        [HasRole("admin,librarian")]
        [HttpPut("{PatronId}", Name = "UpdatePatronUser")]
        public async Task<ActionResult> UpdatePatronUser(Guid PatronId, UpdateUserDto userDto)
        {
            try
            {
                if (PatronId != userDto.Id)
                {
                    return BadRequest();
                }

                var user = userDto.Adapt<User>();
                var query = new UpdateUserByRoleCommand(user, "patron");
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
        [HasRole("admin,librarian")]
        [HttpGet("{PatronId}/BorrowingHistory", Name = "GetPatronBorrwingHistory")]
        public async Task<ActionResult<List<BookTransactionWithStatusDto>>> GetPatronBorrowingHistory(Guid PatronId, int PageNumber = 0, int Count = 5)
        {
            try
            {
                if (Count > 10)
                    Count = 10;
                if (Count < 1)
                    Count = 1;

                var query = new GetPatronBorrowingHistoryQuery(PatronId, PageNumber, Count);
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

    }
}
