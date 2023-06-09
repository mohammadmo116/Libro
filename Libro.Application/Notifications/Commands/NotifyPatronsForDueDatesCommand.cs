using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Application.Notifications.Commands
{
    public class NotifyPatronsForDueDatesCommand:IRequest<bool>
    {
    }
}
