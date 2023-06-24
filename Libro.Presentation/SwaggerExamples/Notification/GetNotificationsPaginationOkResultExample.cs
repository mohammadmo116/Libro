using Libro.Presentation.Dtos.Notifications;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

namespace Libro.Presentation.SwaggerExamples.Notification
{
    public class GetNotificationsPaginationOkResultExample : IExamplesProvider<object>
    {

        public object GetExamples()
        {
            var a = new OkObjectResult(new
            {
                Notifications = new List<GetNotificationsDto>() {
                    new (){
                    Id=Guid.NewGuid(),
                    IsRead=false,
                    Message="Notification Message"
                    },
                       new (){
                    Id=Guid.NewGuid(),
                    IsRead=true,
                    Message="Notification Message2"
                    },
                },
                Pages = 2
            }).Value;
            return a;



        }
    }
}
