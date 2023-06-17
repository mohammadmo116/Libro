namespace Libro.Domain.Exceptions
{
    public class CustomNotFoundException : Exception
    {
        public string _field { get; set; }
        public CustomNotFoundException(string field)
            : base($"404 {field} NotFound")
        {

            _field = field;
        }
    }
}
