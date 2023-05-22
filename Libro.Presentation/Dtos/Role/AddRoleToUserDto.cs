using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace Libro.Presentation.Dtos.Role
{
    public class AddRoleToUserDto
    {
        [Required]
        [MaxLength(450)]
        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }

        [Required]
        [MaxLength(450)]
        [ForeignKey(nameof(Role))]
        public Guid RoleId { get; set; }
    }
}
