using Libro.Application.Authors.Commands;
using Libro.Application.Authors.Queries;
using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using Libro.Domain.Responses;
using Libro.Infrastructure.Authorization;
using Libro.Presentation.Dtos.Author;
using Libro.Presentation.SwaggerExamples.Author;
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
    [Route("Author")]
    [SwaggerResponse(StatusCodes.Status500InternalServerError, "Internal Server Error")]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "Authorization has been denied for this request")]
    [SwaggerResponse(StatusCodes.Status403Forbidden, "When user is not Librarian")]
    public class AuthorController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthorController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get Author's Details by Id
        /// </summary>
        /// <param name="AuthorId"></param>
        /// <returns></returns>
        /// <remarks> 
        /// Sample request:
        ///
        ///     GET /Author/7A0A07DF-C8AE-4407-BD73-9E737D9FBF15 
        /// </remarks>
        [SwaggerResponse(StatusCodes.Status200OK, description: "Returns Author's Detailts", Type = typeof(AuthorDto))]
        [SwaggerResponse(StatusCodes.Status404NotFound, "When Author Is Not Found")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "When user is not Admin,Librarian or Patron")]
        [HasRole("librarian,admin,patron")]
        [HttpGet("{AuthorId}", Name = "GetAuthor")]
        public async Task<ActionResult> GetAuthor(Guid AuthorId)
        {

            var query = new GetAuthorQuery(AuthorId);
            var Result = await _mediator.Send(query);
            if (Result is null)
            {
                var errorResponse = new ErrorResponse(status: HttpStatusCode.NotFound);
                errorResponse.Errors?.Add(new ErrorModel() { FieldName = "Author", Message = "Author Not_Found" });
                return new NotFoundObjectResult(errorResponse);

            }

            return Ok(Result.Adapt<AuthorDto>());

        }

        /// <summary>
        /// Create new author
        /// </summary>
        /// <param name="authorDto"></param>
        /// <returns></returns>
        /// <remarks> 
        /// Sample request:
        /// 
        ///     POST /Author
        ///     {
        ///     name: authorName,
        ///     dateOfBirth: 2023-06-16T16:21:38.124Z
        ///     }
        /// </remarks>

        [SwaggerResponse(StatusCodes.Status201Created, "Returns the newly created Author and it's route in the header:location", Type = typeof(AuthorDto))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "when DateOfBirth is in the future", typeof(ErrorResponse))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(CreateAuthorErrorResponseExample))]
        [HasRole("librarian")]
        [HttpPost(Name = "CreateAuthor")]
        public async Task<ActionResult> CreateAuthor(CreateAuthorDto authorDto)
        {
            var author = authorDto.Adapt<Author>();
            var command = new CreateAuthorCommand(author);
            var Result = await _mediator.Send(command);
            var ResultAuthorDto = Result.Adapt<AuthorDto>();
            return CreatedAtAction(nameof(GetAuthor), new { AuthorId = ResultAuthorDto.Id }, ResultAuthorDto);

        }
        /// <summary>
        ///  Update author by Id
        /// </summary>
        /// <param name="AuthorId"></param>
        /// <param name="authorDto"></param>
        /// <returns></returns>
        /// <remarks> 
        /// Sample request:
        /// 
        ///     PUT /Author/7A0A07DF-C8AE-4407-BD73-9E737D9FBF15
        ///     {
        ///     name: authorName,
        ///     dateOfBirth: 2023-06-16T16:21:38.124Z
        ///     }
        /// </remarks>
        [SwaggerResponse(StatusCodes.Status200OK, "Success when Author is updated")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "When Author is Not Found")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "When AuthorId in the route does not match the one in the body or DateOfBirth is in the future", typeof(UpdateAuthorErrorResponseExample))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(UpdateAuthorErrorResponseExample))]
        [HasRole("librarian")]
        [HttpPut("{AuthorId}", Name = "UpdateAuthor")]
        public async Task<ActionResult> UpdateAuthor(Guid AuthorId, AuthorDto authorDto)
        {
            try
            {
                if (AuthorId != authorDto.Id)
                {
                    var errorResponse = new ErrorResponse(status: HttpStatusCode.BadRequest);
                    errorResponse.Errors?.Add(new ErrorModel() { FieldName = "Id", Message = "bad Id" });
                    return new BadRequestObjectResult(errorResponse);
                }
                var author = authorDto.Adapt<Author>();
                var command = new UpdateAuthorCommand(author);
                var Result = await _mediator.Send(command);
                return Result ? Ok("Author has been Updated") : StatusCode(StatusCodes.Status500InternalServerError);
            }
            catch (CustomNotFoundException e)
            {
                var errorResponse = new ErrorResponse(status: HttpStatusCode.NotFound);
                errorResponse.Errors?.Add(new ErrorModel() { FieldName = "Author", Message = e.Message });
                return new NotFoundObjectResult(errorResponse);

            }
        }
        /// <summary>
        /// Remove Author By Id
        /// </summary>
        /// <param name="AuthorId"></param>
        /// <returns></returns>
        /// <remarks> 
        /// Sample request:
        /// 
        ///     DELETE /Author/3D9402DC-A780-431D-884D-4EE7DDF73FEC 
        /// </remarks>
        [SwaggerResponse(StatusCodes.Status200OK, "Returns Success when Author is deleted")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "When Author is Not Found")]
        [HasRole("librarian")]
        [HttpDelete("{AuthorId}", Name = "RemoveAuthor")]
        public async Task<ActionResult> RemoveAuthor(Guid AuthorId)
        {
            try
            {

                var command = new RemoveAuthorCommand(AuthorId);
                var Result = await _mediator.Send(command);
                return Result ? Ok("Author has been Deleted") : StatusCode(StatusCodes.Status500InternalServerError);
            }
            catch (CustomNotFoundException e)
            {
                var errorResponse = new ErrorResponse(status: HttpStatusCode.NotFound);
                errorResponse.Errors?.Add(new ErrorModel() { FieldName = "Author", Message = e.Message });
                return new NotFoundObjectResult(errorResponse);

            }
        }

    }
}
