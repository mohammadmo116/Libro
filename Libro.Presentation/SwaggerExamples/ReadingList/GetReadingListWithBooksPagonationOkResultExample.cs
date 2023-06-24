using Libro.Presentation.Dtos.Book;
using Libro.Presentation.Dtos.ReadingList;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

namespace Libro.Presentation.SwaggerExamples.ReadingList
{
    public class GetReadingListWithBooksPagonationOkResultExample : IExamplesProvider<object>
    {
        public object GetExamples()
        {
            var a = new OkObjectResult(new
            {
                ReadingList = new GetReadingListWithBooksDto()
                {
                    Id = Guid.NewGuid(),
                    Name = "readingListName",
                    Private = false,
                    Books = new List<BookDto>() {
                       new(){
                       Title="title",
                       Genre="genre",
                       PublishedDate=DateTime.UtcNow.AddYears(-21)
                       },
                        new(){
                       Title="title1",
                       Genre="genre1",
                       PublishedDate=DateTime.UtcNow.AddYears(-10)
                       },
                         new(){
                       Title="title2",
                       Genre="genre2",
                       PublishedDate=DateTime.UtcNow.AddYears(-1)
                       }
                       }
                },


                Pages = 2
            }).Value;
            return a;



        }
    }
}
