using Libro.Presentation.Dtos.BookReview;
using Libro.Presentation.Dtos.User;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

namespace Libro.Presentation.SwaggerExamples.BookReview
{
    public class GetBookReviewsPaginationOkResultExample : IExamplesProvider<object>
    {

        public object GetExamples()
        {
            var user1 = new UserWithIdAndNameDto()
            {
                Id = Guid.NewGuid(),
                UserName = "userName"
            };
            var user2 = new UserWithIdAndNameDto()
            {
                Id = Guid.NewGuid(),
                UserName = "userName"
            };
            var a = new OkObjectResult(new
            {
                Reviews = new List<GetBookReviewWithUserDto>() {
                    new (){
                      BookId=Guid.NewGuid(),
                      Rate=4,
                      Review="stringTest",
                      UserId=user1.Id,
                      User = user1

                     },
                      new (){
                      BookId=Guid.NewGuid(),
                      Rate=4,
                      Review="stringTest2",
                      UserId=user2.Id,
                      User = user2

                     },

                },
                Pages = 2
            }).Value;
            return a;



        }

    }
}
