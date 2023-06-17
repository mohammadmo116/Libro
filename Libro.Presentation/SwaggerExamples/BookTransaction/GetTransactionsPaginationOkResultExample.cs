using Libro.Domain.Enums;
using Libro.Domain.Responses;
using Libro.Presentation.Dtos.Book;
using Libro.Presentation.Dtos.BookReview;
using Libro.Presentation.Dtos.BookTransaction;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Presentation.SwaggerExamples.BookTransaction
{
    public class GetTransactionsPaginationOkResultExample : IExamplesProvider<object>    
    {

        public object GetExamples()
        {
            var a = new OkObjectResult(new
            {
                Transactions = new List<BookTransactionWithStatusDto>() {
                    new (){
                        Status=BookStatus.Borrowed,
                        DueDate=DateTime.UtcNow.AddDays(-5),
                        Book=new CreateBookDto(){
                        Title="title",
                        Genre="genre",
                        IsAvailable=false,
                        PublishedDate=DateTime.UtcNow.AddDays(-150),
                        },

                    },
                       new (){
                        Status=BookStatus.Borrowed,
                        DueDate=DateTime.UtcNow.AddDays(-10),
                        Book=new CreateBookDto(){
                        Title="title1",
                        Genre="genre1",
                        IsAvailable=false,
                        PublishedDate=DateTime.UtcNow.AddDays(-300),
                        },

                    },


                }, 
                Pages = 2
            }).Value;
            return a;



        }

    }
}
