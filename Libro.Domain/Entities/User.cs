using Libro.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Domain.Entities
{
    public class User : BaseEntity
    {
       
        [MaxLength(256)]
        public string? UserName { get; set; }
        [Required]
        [MaxLength(256)]
        [EmailAddress]
        public string Email { get; set; }=String.Empty;

        [PasswordPropertyText]
        [Required]
        public string Password { get; set; }
        [Phone]
        public string? PhoneNumber { get; set; }
    
        public List<Role> Roles { get; set; }
    }
}
