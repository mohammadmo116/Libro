namespace Libro.Domain.Exceptions
{
    public class EmailExistsException : UserExistsException
    {
        public EmailExistsException(string email)
            : base("Email")
        {
        }
    }
}
