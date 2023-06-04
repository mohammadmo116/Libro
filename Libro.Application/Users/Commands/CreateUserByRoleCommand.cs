using Libro.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Application.Users.Commands
{
    public sealed record CreateUserByRoleCommand(User User,string RoleName):IRequest<User>;
}
