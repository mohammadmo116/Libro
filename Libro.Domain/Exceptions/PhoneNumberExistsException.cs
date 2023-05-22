namespace Libro.Domain.Exceptions
{
    public class PhoneNumberExistsException : UserExistsException
    {
        public PhoneNumberExistsException(string PhoneNumber)
            : base("PhoneNumber")
        {
        }
    }
}
