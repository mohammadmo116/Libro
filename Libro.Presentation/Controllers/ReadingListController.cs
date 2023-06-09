using Libro.Application.Books.Commands;
using Libro.Application.Books.Queries;
using Libro.Application.ReadingLists.Commands;
using Libro.Application.ReadingLists.Queries;
using Libro.Domain.Entities;
using Libro.Domain.Exceptions;
using Libro.Domain.Responses;
using Libro.Infrastructure.Authorization;
using Libro.Presentation.Dtos.Book;
using Libro.Presentation.Dtos.ReadingList;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Presentation.Controllers
{
    [Route("ReadingList")]
    [ApiController]
    public class ReadingListController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ReadingListController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HasRole("patron")]
        [HttpGet("{ReadingListId}/Books", Name = "GetReadingListWithBooks")]
        public async Task<ActionResult<(GetReadingListWithBooksDto ,int)>> GetReadingListWithBooks(Guid ReadingListId, int PageNumber=0, int Count = 5)
        {
            if(Count>10)
                Count=10;
            if (Count < 1)
                Count = 1;

            string? userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userId, out Guid parsedUserId))
            {
                return Forbid();
            }
            var query = new GetReadingListWithBooksQuery(parsedUserId,ReadingListId, PageNumber, Count);
            var Result = await _mediator.Send(query);

            if (Result.Item1 is null)
                return NotFound("ReadingList Not_Found");
            
            return Ok(new {
                ReadingList= Result.Item1.Adapt<GetReadingListWithBooksDto>(),
                Pages=Result.Item2 }
            );


        }
        [HasRole("patron")]
        [HttpPost(Name = "CreateReadingList")]
        public async Task<ActionResult> CreateReadingList(CreateReadingListDto readingListDto)
        {

            string? userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userId, out Guid parsedUserId))
            {
                return Forbid();
            }
            var readingList = readingListDto.Adapt<ReadingList>();
            var command = new CreateReadingListCommand(parsedUserId, readingList);
            var Result = await _mediator.Send(command);
            var bookWithAuthorsDto = Result.Adapt<GetReadingListDto>();
            return CreatedAtAction(nameof(GetReadingListWithBooks), new { ReadingListId = bookWithAuthorsDto.Id }, bookWithAuthorsDto);



        }
        [HasRole("patron")]
        [HttpPut("{ReadingListId}", Name = "UpdateReadingList")]
        public async Task<ActionResult> UpdateReadingList(Guid ReadingListId, UpdateReadingListDto readingListDto)
        {
            try
            {
                string? userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
                if (!Guid.TryParse(userId, out Guid parsedUserId))
                {
                    return Forbid();
                }
                if (ReadingListId != readingListDto.Id)
                {
                    return BadRequest("wrong Id");
                }
                var readingList = readingListDto.Adapt<ReadingList>();
                var command = new UpdateReadingListCommand(parsedUserId,readingList);
                var Result = await _mediator.Send(command);
                return Result ? Ok("ReadingList has been Updated") : StatusCode(StatusCodes.Status500InternalServerError);
            }
            catch (CustomNotFoundException e)
            {
                var errorResponse = new ErrorResponse(status: HttpStatusCode.NotFound);
                errorResponse.Errors?.Add(new ErrorModel() { FieldName = "ReadingList", Message = e.Message });
                return new BadRequestObjectResult(errorResponse);

            }
        }
        [HasRole("patron")]
        [HttpDelete("{ReadingListId}", Name = "RemoveReadingList")]
        public async Task<ActionResult> RemoveReadingList(Guid ReadingListId)
        {
            try
            {
                string? userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
                if (!Guid.TryParse(userId, out Guid parsedUserId))
                {
                    return Forbid();
                }
                var command = new RemoveReadingListCommand(parsedUserId,ReadingListId);
                var Result = await _mediator.Send(command);
                return Result ? Ok("Reading List has been Deleted") : StatusCode(StatusCodes.Status500InternalServerError);
            }
            catch (CustomNotFoundException e)
            {
                var errorResponse = new ErrorResponse(status: HttpStatusCode.NotFound);
                errorResponse.Errors?.Add(new ErrorModel() { FieldName = "Reading List", Message = e.Message });
                return new BadRequestObjectResult(errorResponse);

            }
        }

    }
}
