using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Application.Users.Commands
{
    public sealed record RemoveUserByRoleCommand(Guid UserId,string RoleName) : IRequest<bool>;
   
}
