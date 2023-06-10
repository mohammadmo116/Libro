namespace Libro.Domain.Exceptions.BookExceptions
{
    public class NotAllowedToReviewBookException : Exception
    {
        public NotAllowedToReviewBookException()
            : base("Not allowed to review book, you need to borrow and return the book")
        {
        }
    }
}
