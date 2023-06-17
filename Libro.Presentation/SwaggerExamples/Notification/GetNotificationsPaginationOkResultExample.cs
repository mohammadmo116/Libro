using Libro.Domain.Enums;
using Libro.Presentation.Dtos.Book;
using Libro.Presentation.Dtos.BookTransaction;
using Libro.Presentation.Dtos.Notifications;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
