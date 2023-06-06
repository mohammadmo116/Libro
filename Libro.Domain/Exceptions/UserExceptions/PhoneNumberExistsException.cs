namespace Libro.Domain.Exceptions.UserExceptions
{
    public class PhoneNumberExistsException : UserExistsException
    {
        public PhoneNumberExistsException(string PhoneNumber)
            : base("PhoneNumber")
        {
        }
    }
}
