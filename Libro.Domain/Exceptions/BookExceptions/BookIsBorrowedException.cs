namespace Libro.Domain.Exceptions.BookExceptions
{
    public class BookIsBorrowedException : Exception
    {
        public BookIsBorrowedException() : base("book is already Borrowed")
        {
        }

    }
}
