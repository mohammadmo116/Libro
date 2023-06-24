using Libro.Domain.Entities;

namespace Libro.Infrastructure.Repositories
{
    public interface IReadingListRepository
    {
        Task<(List<ReadingList>, int)> GetReadingListsByUserAsync(Guid UserId, int PageNumber, int Count);
        Task<(ReadingList, int)> GetReadingListWithBooksAsync(Guid UserId, Guid ReadingListId, int PageNumber, int Count);
        Task<ReadingList> GetReadingListByUserAsync(Guid UserId, Guid ReadingListId);
        Task CreateReadingListAsync(ReadingList readingList);
        void UpdateReadingList(ReadingList readingList);
        void RemoveReadingList(ReadingList readingList);
        Task AddBookToReadingList(BookReadingList bookReadingList);
        void RemoveBookFromReadingList(BookReadingList bookReadingList);
        Task<bool> ContainsTheBook(BookReadingList bookReadingList);

    }
}