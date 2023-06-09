using Libro.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Domain.Entities
{
    public class Notification : BaseEntity
    {
        [Required]
        public string Message { get; set; }
        public Guid? UserId { get; set; }
        public User? User { get; set; }
    }
}
