namespace Libro.Domain.Exceptions
{

    public class RoleExistsException : Exception
    {

        public RoleExistsException(string RoleName)
           : base($"The Role With Name {RoleName.ToLower()} Already Exists")
        {
        }


    }
}