using Libro.Application.Books.Commands;
using Libro.Application.Books.Queries;
using Libro.Application.ReadingLists.Commands;
using Libro.Application.ReadingLists.Queries;
using Libro.Domain.Entities;
using Libro.Infrastructure.Authorization;
using Libro.Presentation.Dtos.Book;
using Libro.Presentation.Dtos.ReadingList;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
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
        [HttpGet("{ReadingListId}", Name = "GetReadingListWithBooks")]
        public async Task<ActionResult<(ReadingListWithBooksDto ,int)>> GetReadingListWithBooks(Guid ReadingListId, int PageNumber=0, int Count = 5)
        {
            if(Count>10)
                Count=10;
            if (Count < 1)
                Count = 1;

            var query = new GetReadingListWithBooksQuery(ReadingListId,PageNumber, Count);
            var Result = await _mediator.Send(query);

            if (Result.Item1 is null)
                return NotFound("ReadingList Not_Found");
            
            return Ok(new {
                ReadingList= Result.Item1.Adapt<ReadingListWithBooksDto>(),
                Pages=Result.Item2 }
            );


        }
        [HasRole("patron")]
        [HttpPost(Name = "CreateReadingList")]
        public async Task<ActionResult> CreateReadingList(CreateReadingListDto readingListDto)
        {

            string? userId = User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Sid)?.Value;
            if (!Guid.TryParse(userId, out Guid parsedUserId))
            {
                return Forbid();
            }
            var readingList = readingListDto.Adapt<ReadingList>();
            var command = new CreateReadingListCommand(parsedUserId, readingList);
            var Result = await _mediator.Send(command);
            var bookWithAuthorsDto = Result.Adapt<ReadingListDto>();
            return CreatedAtAction(nameof(GetReadingListWithBooks), new { ReadingListId = bookWithAuthorsDto.Id }, bookWithAuthorsDto);



        }

    }
}
