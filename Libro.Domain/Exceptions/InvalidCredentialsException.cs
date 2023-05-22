namespace Libro.Domain.Exceptions
{
    public class InvalidCredentialsException:Exception
    {
        public InvalidCredentialsException(string email,string Password)
            : base($"Invalid Credentials,\n email : {email} \n password: {Password}")
        {
        }
    }
}
