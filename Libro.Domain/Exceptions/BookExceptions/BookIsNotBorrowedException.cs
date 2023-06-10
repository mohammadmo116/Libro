namespace Libro.Domain.Exceptions.BookExceptions
{
    public class BookIsNotBorrowedException : Exception
    {
        public BookIsNotBorrowedException(string title) : base($"Book {title} is not Borrowed")
        {
        }
    }
}
