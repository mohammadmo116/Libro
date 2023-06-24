using Libro.Presentation.Dtos.ReadingList;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

namespace Libro.Presentation.SwaggerExamples.ReadingList
{
    public class GetReadingListPaginationOkResultExample : IExamplesProvider<object>
    {
        public object GetExamples()
        {
            var a = new OkObjectResult(new
            {
                ReadingLists = new List<GetReadingListDto>()
                {
                    new()
                    {
                        Id = Guid.NewGuid(),
                        Name = "readingListName1",
                        Private = false,
                    },
                     new()
                     {
                        Id = Guid.NewGuid(),
                        Name = "readingListName2",
                        Private = true,
                     }

                },
                Pages = 2
            }).Value;
            return a;



        }
    }
}
