namespace Libro.Domain.Exceptions.UserExceptions
{
    public class EmailExistsException : UserExistsException
    {
        public EmailExistsException(string email)
            : base("Email")
        {
        }
    }
}
