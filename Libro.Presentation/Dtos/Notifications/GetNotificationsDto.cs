﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Presentation.Dtos.Notifications
{
    public class GetNotificationsDto
    {
        public Guid Id { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }

    }
}
