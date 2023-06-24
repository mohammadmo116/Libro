using Libro.Domain.Enums;
using Libro.Presentation.Dtos.BookTransaction;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

namespace Libro.Presentation.SwaggerExamples.Patron
{
    public class GetPatronBorrowingHistoryOkResponseExample : IExamplesProvider<object>
    {
        public object GetExamples()
        {
            var a = new OkObjectResult(new
            {
                Transactions = new List<BookTransactionWithStatusDto>() {
                        new (){
                            DueDate = DateTime.Now,
                            Status=BookStatus.Borrowed,
                            Book=new (){
                            Title="test",
                            Genre="genre",
                            IsAvailable=false,
                            PublishedDate=DateTime.Now.AddDays(-120)
                            },
                        },
                         new (){
                            DueDate = DateTime.Now,
                            Status=BookStatus.Reserved,
                            Book=new (){
                            Title="test1",
                            Genre="genre1",
                            IsAvailable=false,
                            PublishedDate=DateTime.Now.AddDays(-130)
                            },
                        }
                    },


                Pages = 2
            }).Value;
            return a;



        }
    }
}
