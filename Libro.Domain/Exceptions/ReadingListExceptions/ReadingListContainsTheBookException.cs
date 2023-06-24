namespace Libro.Domain.Exceptions.ReadingListExceptions
{
    public class ReadingListContainsTheBookException : Exception
    {
        public ReadingListContainsTheBookException() : base("ReadingList Already Has The Specific Book")
        {

        }

    }
}
