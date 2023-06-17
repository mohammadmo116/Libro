using Libro.Domain.Enums;
using Libro.Domain.Responses;
using Libro.Presentation.Dtos.Book;
using Libro.Presentation.Dtos.BookTransaction;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Presentation.SwaggerExamples.Book
{
    public class GetBookPaginationOkResultExample : IExamplesProvider<object>
    {
        public object GetExamples()
        {
            var a = new OkObjectResult(new
            {
                Books = new List<GetBookDto>() {
                    new (){
                        Id=Guid.NewGuid(),
                        Genre="genre",
                        PublishedDate=DateTime.Now.AddDays(-120),
                        Title="titleTest"
                     },
                        new (){
                        Id=Guid.NewGuid(),
                        Genre="genre1",
                        PublishedDate=DateTime.Now.AddDays(-130),
                        Title="titleTest1"
                     },
                },
                Pages = 2
            }).Value;
            return a;



        }
    }
}
