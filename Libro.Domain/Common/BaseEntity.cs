using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Libro.Domain.Common
{
    public abstract class BaseEntity
    {
      
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

    }
}
