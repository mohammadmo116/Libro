using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Domain.Entities
{

    public class UserRole
    {
        [Required]
        [MaxLength(450)]
        [ForeignKey(nameof(Entities.User))]
        public Guid UserId { get; set; }

        [Required]
        [MaxLength(450)]
        [ForeignKey(nameof(Entities.Role))]
        public Guid RoleId { get; set; }
        
    }
}
