namespace Libro.Domain.Exceptions.UserExceptions
{
    public class UserNameExistsException : UserExistsException
    {
        public UserNameExistsException(string UserName)
            : base("UserName")
        {
        }
    }
}
