namespace Libro.Domain.Exceptions
{
    public class CustomNotFoundException : Exception
    {
        public CustomNotFoundException(string code)
            : base($"404 {code} NotFound")
        {
        }
    }
}
