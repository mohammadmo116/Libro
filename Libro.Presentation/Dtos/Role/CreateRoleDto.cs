using Libro.Domain.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Presentation.Dtos.Role
{
    public class CreateRoleDto
    {
        public Guid Id { get; set; }
        [Required]
        [MaxLength(256)]
        public string Name { get; set; }
    }
}
