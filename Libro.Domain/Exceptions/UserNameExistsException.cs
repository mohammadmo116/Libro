namespace Libro.Domain.Exceptions
{
    public class UserNameExistsException : UserExistsException
    {
        public UserNameExistsException(string UserName)
            : base("UserName")
        {
        }
    }
}
