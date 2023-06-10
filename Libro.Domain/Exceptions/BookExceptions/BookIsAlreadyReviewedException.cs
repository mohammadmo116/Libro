namespace Libro.Domain.Exceptions.BookExceptions
{
    public class BookIsAlreadyReviewedException : Exception
    {
        public BookIsAlreadyReviewedException() : base("you Already have Reviewed this book") { }
    }
}
