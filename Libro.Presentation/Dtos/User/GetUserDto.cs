using Libro.Domain.Common;
using Libro.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Libro.Presentation.Dtos.User
{
    public class GetUserDto : UserDto
    {

        public Guid Id { get; set; }


    }
}
