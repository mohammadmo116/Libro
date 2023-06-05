﻿using Libro.Domain.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Application.Users.Commands
{
    public sealed record UpdateUserByRoleCommand(User user,string RoleName):IRequest<bool>;
   
}