using Libro.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Libro.Domain.Entities
{
    public class Role : BaseEntity
    {
       
        [Required]
        [MaxLength(256)]
        public string Name { get; set; }
        public List<User> Users { get; set; }
    }
}
