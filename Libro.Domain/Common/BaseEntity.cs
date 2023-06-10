using System.ComponentModel.DataAnnotations.Schema;

namespace Libro.Domain.Common
{
    public abstract class BaseEntity
    {

        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid Id { get; set; }

    }
}
