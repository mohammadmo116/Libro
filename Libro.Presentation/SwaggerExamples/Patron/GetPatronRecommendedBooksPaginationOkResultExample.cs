using Libro.Presentation.Dtos.Author;
using Libro.Presentation.Dtos.Book;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

namespace Libro.Presentation.SwaggerExamples.Patron
{
    public class GetPatronRecommendedBooksPaginationOkResultExample : IExamplesProvider<object>
    {
        public object GetExamples()
        {
            var a = new OkObjectResult(new
            {
                RecommendedBooks = new List<BookWithAuthorsDto>() {
                        new (){
                          Id=Guid.NewGuid(),
                          Genre="genre",
                          PublishedDate=DateTime.UtcNow.AddDays(-320),
                          Title="title",
                          Authors=new List<AuthorDto>(){
                                new(){
                                    Id=Guid.NewGuid(),
                                    Name="name",
                                    DateOfBirth=DateTime.UtcNow.AddYears(-20)
                                },
                                  new(){
                                    Id=Guid.NewGuid(),
                                    Name="name1",
                                    DateOfBirth=DateTime.UtcNow.AddYears(-25)
                                },
                            }

                        },
                             new (){
                          Id=Guid.NewGuid(),
                          Genre="genre2",
                          PublishedDate=DateTime.UtcNow.AddDays(-150),
                          Title="title2",
                          Authors=new List<AuthorDto>(){
                                new(){
                                    Id=Guid.NewGuid(),
                                    Name="name2",
                                    DateOfBirth=DateTime.UtcNow.AddYears(-27)
                                },
                                  new(){
                                    Id=Guid.NewGuid(),
                                    Name="name3",
                                    DateOfBirth=DateTime.UtcNow.AddYears(-30)
                                },
                            }

                        }

                    },


                Pages = 2
            }).Value;
            return a;



        }
    }
}
