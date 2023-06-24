namespace Libro.Domain.Exceptions.ReadingListExceptions
{
    public class ReadingListDoesNotContainTheBookException : Exception
    {
        public ReadingListDoesNotContainTheBookException() : base("ReadingList Does Not Contain The Book")
        {
        }
    }
}
