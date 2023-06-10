using Libro.Domain.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Libro.Domain.Entities
{
    public class User : BaseEntity
    {


        [Required]
        [MaxLength(256)]
        public string UserName { get; set; }
        [Required]
        [MaxLength(256)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [PasswordPropertyText]
        [Required]
        public string Password { get; set; }
        [Phone]
        public string? PhoneNumber { get; set; }

        public List<Role>? Roles { get; set; } = new();
        public List<Book>? Books { get; set; } = new();
        public List<BookTransaction>? BookTransactions { get; set; } = new();
        public List<ReadingList>? ReadingLists { get; set; }
        public List<BookReview>? Reviews { get; set; }
        public List<Notification>? Notifications { get; set; }
    }
}
