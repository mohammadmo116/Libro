using Libro.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Application.Notifications.Queries
{
    public sealed record class GetUserNotificaionsQuery(Guid UserId,int PageNumbr, int Count):IRequest<(List<Notification>,int)>;
   
}
