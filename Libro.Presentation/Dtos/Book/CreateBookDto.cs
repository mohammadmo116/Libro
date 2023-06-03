﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Presentation.Dtos.Book
{
    public class CreateBookDto
    {
        [Required]
        [MaxLength(256)]
        public string? Title { get; set; }
        public string? Genre { get; set; }
        public bool IsAvailable { get; set; }
        public DateTime? PublishedDate { get; set; }
    }
}
