﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Presentation.Dtos.Author
{
    public class AuthorDto : CreateAuthorDto
    {
        public Guid Id { get; set; }
    }
}
