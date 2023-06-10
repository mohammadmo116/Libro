namespace Libro.Domain.Exceptions.BookExceptions
{
    public class BookIsNotAvailableException : Exception

    {
        public BookIsNotAvailableException(string title) : base($"the Book {title} is not avaialble at the moment")
        {
        }
    }
}